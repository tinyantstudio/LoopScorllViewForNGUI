using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
    /// maybe two more item.
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

    private bool isInitLoopManager = false;

    // 
    private GameObject cacheUnuseItemParent = null;

    void Awake()
    {
        this.cacheUnuseItemParent = new GameObject("[CacheUnUsedItemParent]");
        this.cacheUnuseItemParent.transform.parent = this.mScrollView.gameObject.transform;
        this.cacheUnuseItemParent.transform.localScale = Vector3.one;
        this.cacheUnuseItemParent.transform.localPosition = new Vector3(100000f, 0f, 0f);
        this.cacheUnuseItemParent.transform.localRotation = Quaternion.identity;
        this.cacheUnuseItemParent.SetActive(false);
        this.mGrid.hideInactive = false;
    }

    private void MoveItemToCacheParent(Transform trsTarget)
    {
        trsTarget.parent = this.cacheUnuseItemParent.transform;
        trsTarget.localPosition = Vector3.zero;
    }

    private void MoveBackToGrid(Transform trsTarget)
    {
        trsTarget.parent = this.mGrid.transform;
    }

    private void ShowLoopScrollView()
    {
        if (this.mScrollView != null)
            mPanel = this.mScrollView.GetComponent<UIPanel>();
        this.mTrans = mGrid.transform;
        // reset the scroll view.
        isInitLoopManager = true;
        ResetScrollView();
    }

    // when the data change refresh the item data.
    // 1. delete item data.
    // 2. add item data.
    public void RefreshLoopScrollView()
    {
        if (!this.isInitLoopManager)
        {
            this.ShowLoopScrollView();
            return;
        }
        Debuger.Log("@refresh loop scroll view. " + this.itemDatas.Count);
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

        // set the grid sort method
        // 设置grid的排序规则，防止出现排序位置显示错误
        this.mGrid.sorting = UIGrid.Sorting.Alphabetic;

        for (int i = 0; i < realItemIndex; i++)
        {
            // get item from the free gameobject list
            // GameObject obj = GameObject.Instantiate(this.prefab);
            GameObject obj = this.GetFreeItem();
            if (obj == null)
                obj = GameObject.Instantiate(this.prefab);
            if (!obj.activeSelf)
                obj.SetActive(true);

            this.AddChildToTarget(mGrid.transform, obj.transform);
            UIWidget widget = obj.GetComponent<UIWidget>();
            this.mChildren.Add(widget);
            this.mChildrenStatus[widget] = false;

            // 设置排序规则保证第一次显示顺序一致
            // set the item right name to sort right
            //obj.name = i.ToString();

            StringBuilder sb = new StringBuilder();
            if (i <= 9)
                obj.name = sb.AppendFormat("A{0}", i).ToString();
            else if (i > 9)
                obj.name = sb.AppendFormat("B{0}", i).ToString();

            LoopBaseItem itemScript = obj.GetComponent<LoopBaseItem>();
            itemScript.CurItemIndex = i;
            itemScript.UpdateData(this.itemDatas[i]);
            this.itemScripts.Add(itemScript);
        }
        mGrid.Reposition();
        this.mScrollView.ResetPosition();
        this.UpdateItemVisable(true);
        Debuger.Log("@view item count is " + this.itemDatas.Count);
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
        // Debuger.Log(" current max index is " + this.curMaxDataIndex);
        // Debuger.Log(" current min index is " + this.curMinDataIndex);
    }


    private List<GameObject> freeItemList = new List<GameObject>();
    private void ResetScrollView()
    {
        int childCount = this.mGrid.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            // never destroy the item just move to cached parent transform.
            // it's not right to Instantiate and destroy item so often
            // NOT setactive(false) and setactive(true) just move to very cool far
            // because the SetActive will cost much,you can compare SetActive to move far by looking
            // profiler connect with your device

            // 不直接销毁item，存放起来重复使用减少内存和CPU开销
            // Destroy(trs.gameObject);
            // 别用SetActive(false)隐藏，这样会造成较大的开销，直接的移动到一个很遥远地方，不让你的
            // item从面板中实际清除
            // 你可以用过profiler调试对比下两者之间的区别

            Transform trs = this.mGrid.transform.GetChild(i);
            this.MoveItemToCacheParent(trs);
            freeItemList.Add(trs.gameObject);
        }

        // auto set the min loop item count.
        this.CheckMinLoopItemCount();

        this.mGrid.transform.DetachChildren();
        this.mGrid.Reposition();
        this.InitLoopScrollView();
    }

    private GameObject GetFreeItem()
    {
        if (this.freeItemList.Count <= 0)
            return null;
        GameObject obj = this.freeItemList[0];
        this.freeItemList.Remove(obj);
        this.MoveBackToGrid(obj.transform);
        return obj;
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
        Debuger.Log("@ min loop item count is " + this.minLoopCount);
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
