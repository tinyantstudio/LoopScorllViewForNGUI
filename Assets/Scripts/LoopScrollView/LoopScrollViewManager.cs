using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoopScrollViewManager : MonoBehaviour
{
    public UIScrollView mScrollView;
    public UIGrid mGrid;
    private UIPanel mPanel;
    private Transform mTrans;

    [HideInInspector]
    public List<UIWidget> mChildren;
    public Dictionary<UIWidget, bool> mChildrenStatus = new Dictionary<UIWidget, bool>();

    /// <summary>
    /// you should choose the appropriate item count,
    /// two more item.
    /// </summary>
    private int minLoopCount = 7;
    [HideInInspector]
    public int curMinDataIndex = 0;
    [HideInInspector]
    public int curMaxDataIndex = 0;
    public GameObject prefab;
    public bool isInLoop = true;
    private List<LoopBaseItem> itemScripts = new List<LoopBaseItem>();
    // is vertical move.
    public bool isVertical = false;
    public List<object> itemDatas = new List<object>();

    public void ShowLoopScrollView()
    {
        if (this.mScrollView != null)
            mPanel = this.mScrollView.GetComponent<UIPanel>();
        this.mTrans = mGrid.transform;
        // reset the scroll view.
        ResetScrollView();
    }

    // when the data change refresh the item data.
    // 1. delete item data.
    // 2. add item data.
    public void RefreshLoopScrollView()
    {
        Debug.Log("@refresh loop scroll view. " + this.itemDatas.Count);
        if (this.itemDatas.Count >= this.minLoopCount && !this.isInLoop)
            this.ResetScrollView();
        if (this.itemDatas.Count < this.minLoopCount)
            this.ResetScrollView();

        // in loop just update.
        if (this.isInLoop)
        {
            int dataCount = this.itemDatas.Count;
            this.itemScripts.Sort(this.SortByCurrentDataIndex);
            if (this.itemScripts[0].CurItemIndex >= dataCount)
            {
                // reset the max and min index.
                this.curMaxDataIndex = dataCount - 1;
                this.curMinDataIndex = this.curMaxDataIndex - this.minLoopCount + 1;
            }
            for (int i = 0; i < this.itemScripts.Count; i++)
            {
                int curItemIndex = this.curMaxDataIndex - i;
                this.itemScripts[i].CurItemIndex = curItemIndex;
                this.itemScripts[i].UpdateData(this.itemDatas[curItemIndex]);
            }
        }
    }

    void LateUpdate()
    {
        if (this.isInLoop)
            this.UpdateItemVisable(false);
    }

    private void InitLoopScrollView()
    {
        int realItemIndex = this.minLoopCount;
        this.itemScripts.Clear();
        this.mChildren.Clear();

        if (this.minLoopCount > this.itemDatas.Count)
        {
            // just add child.
            realItemIndex = this.itemDatas.Count;
            isInLoop = false;

            this.curMaxDataIndex = this.itemDatas.Count;
            this.curMinDataIndex = 0;
        }
        else
        {
            this.curMinDataIndex = 0;
            this.curMaxDataIndex = this.minLoopCount - 1;
            this.isInLoop = true;
        }

        for (int i = 0; i < realItemIndex; i++)
        {
            GameObject obj = GameObject.Instantiate(this.prefab);
            this.AddChildToTarget(mGrid.transform, obj.transform);
            UIWidget widget = obj.GetComponent<UIWidget>();
            this.mChildren.Add(widget);
            this.mChildrenStatus[widget] = false;
            obj.name = i.ToString();
            LoopBaseItem itemScript = obj.GetComponent<LoopBaseItem>();
            itemScript.CurItemIndex = i;
            itemScript.UpdateData(this.itemDatas[i]);
            this.itemScripts.Add(itemScript);
        }
        mGrid.Reposition();
        this.mScrollView.ResetPosition();
        this.UpdateItemVisable(true);
        Debug.Log("@view item count is " + this.itemDatas.Count);
    }

    private void UpdateItemVisable(bool isFirstTime)
    {
        for (int i = 0; i < this.mChildren.Count; i++)
        {
            UIWidget widget = this.mChildren[i];
            bool isVisable = this.mPanel.IsVisible(widget);
            if (!isFirstTime)
            {
                if (this.mChildrenStatus[widget] != isVisable
                    && isVisable == false)
                {
                    if (OnChangeItemVisable(widget.transform))
                        return;
                }
            }
            this.mChildrenStatus[widget] = isVisable;
        }
    }

    private bool OnChangeItemVisable(Transform trs)
    {
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
        bool dragToEnd = (!this.isVertical ? trs.localPosition.x < center.x : trs.localPosition.y > center.y);
        bool needToChangeItem = dragToEnd ? (this.curMaxDataIndex < this.itemDatas.Count - 1) : (this.curMinDataIndex > 0);

        if (needToChangeItem)
            return TryChangeItem(trs, dragToEnd);
        else
            this.mScrollView.restrictWithinPanel = true;
        return false;
    }

    private bool TryChangeItem(Transform trs, bool isToEnd)
    {
        this.mScrollView.restrictWithinPanel = false;
        List<Transform> moveTargets = new List<Transform>();
        moveTargets.Add(trs);
        Transform realMoveTrs = null;
        float fMinPositionX = (this.isVertical ? trs.localPosition.y : trs.localPosition.x);
        for (int i = 0; i < this.minLoopCount; i++)
        {
            if (this.mChildren[i].transform != trs
                && this.mChildren[i].gameObject.activeSelf)
            {
                float posValue = (this.isVertical ? this.mChildren[i].transform.localPosition.y : this.mChildren[i].transform.localPosition.x);
                bool addToMoveTargetList = isToEnd ? (!this.isVertical ? posValue < fMinPositionX : posValue > fMinPositionX) : (!this.isVertical ? posValue > fMinPositionX : posValue < fMinPositionX);
                if (addToMoveTargetList)
                {
                    fMinPositionX = posValue;
                    moveTargets.Add(this.mChildren[i].transform);
                    realMoveTrs = this.mChildren[i].transform;
                }
            }
        }
        if (moveTargets.Count >= 2)
        {
            this.ChangeItemLocation(realMoveTrs, isToEnd);
            return true;
        }
        return false;
    }

    private void ChangeItemLocation(Transform targetTrs, bool isToEnd)
    {
        Vector3 newPos = targetTrs.localPosition;
        int flag = isToEnd ? 1 : -1;

        if (!this.isVertical)
            newPos.x += (this.mGrid.cellWidth * (this.minLoopCount)) * flag;
        else
            newPos.y -= (this.mGrid.cellHeight * (this.minLoopCount)) * flag;
        targetTrs.localPosition = newPos;
        this.mScrollView.InvalidateBounds();
        this.UpdateItemData(isToEnd, targetTrs);
    }

    private void UpdateItemData(bool isToEnd, Transform targetTrs)
    {
        // update the item 
        if (isToEnd)
        {
            this.curMaxDataIndex++;
            LoopBaseItem itemScript = targetTrs.GetComponent<LoopBaseItem>();
            this.curMinDataIndex = itemScript.CurItemIndex + 1;

            itemScript.CurItemIndex = this.curMaxDataIndex;
            itemScript.UpdateData(this.itemDatas[this.curMaxDataIndex]);
        }
        else
        {
            this.curMinDataIndex--;
            LoopBaseItem itemScript = targetTrs.GetComponent<LoopBaseItem>();
            this.curMaxDataIndex = itemScript.CurItemIndex - 1;

            itemScript.CurItemIndex = this.curMinDataIndex;
            itemScript.UpdateData(this.itemDatas[this.curMinDataIndex]);
        }
        // Debug.Log(" current max index is " + this.curMaxDataIndex);
        // Debug.Log(" current min index is " + this.curMinDataIndex);
    }

    private void ResetScrollView()
    {
        int childCount = this.mGrid.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform trs = this.mGrid.transform.GetChild(i);
            Destroy(trs.gameObject);
        }

        // auto set the min loop item count.
        this.CheckMinLoopItemCount();

        this.mGrid.transform.DetachChildren();
        this.mGrid.Reposition();
        this.InitLoopScrollView();
    }

    private void CheckMinLoopItemCount()
    {
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }
        int calCount = 0;
        if (this.isVertical)
        {
            float length = Mathf.Abs(corners[0].y - corners[2].y);
            calCount = Mathf.CeilToInt(length / mGrid.cellHeight);
        }
        else
        {
            float width = Mathf.Abs(corners[0].x - corners[2].x);
            calCount = Mathf.CeilToInt(width / mGrid.cellWidth);
        }
        this.minLoopCount = (calCount + 2);
        Debug.Log("@ min loop item count is " + this.minLoopCount);
    }

    private int SortByCurrentDataIndex(LoopBaseItem left, LoopBaseItem right)
    {
        if (left.CurItemIndex > right.CurItemIndex)
            return -1;
        else if (left.CurItemIndex < right.CurItemIndex)
            return 1;
        else
            return 0;
    }

    //////// 辅助函数，移动到工具脚本文件内即可 ////////
    //////// Helper functions just move to other script file.
    /// <summary>
    /// 添加子节点 Add new child to target.
    /// </summary>
    private void AddChildToTarget(Transform target, Transform child)
    {
        child.parent = target;
        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = Vector3.zero;

        ChangeChildLayer(child, target.gameObject.layer);
    }

    /// <summary>
    /// 修改子节点Layer  NGUITools.SetLayer();
    /// </summary>
    private void ChangeChildLayer(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            ChangeChildLayer(child, layer);
        }
    }
}
