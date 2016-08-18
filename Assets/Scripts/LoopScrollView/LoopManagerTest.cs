using UnityEngine;
using System.Collections;

public class LoopManagerTest : MonoBehaviour
{
    public int dataCount = 100;

    public bool addItem = false;
    public int perAddItemCount = 1;

    public GameObject btnAddItem;
    public GameObject btnRemoveItem;
    public GameObject btnRefresh;

    private static int addCount = 1;

    // [Header("refresh ScrollView new data count")]
    private int newDataCount = 5;
    public UIInput inputNewDataCount;

    LoopScrollViewManager scrollViewManager = null;

    void Start()
    {
        scrollViewManager = this.GetComponent<LoopScrollViewManager>();
        scrollViewManager.itemDatas.Clear();
        for (int i = 0; i < dataCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = i + 1;
            scrollViewManager.itemDatas.Add(data);
        }
        scrollViewManager.RefreshLoopScrollView();

        // set button listener.
        UIEventListener.Get(btnAddItem).onClick = this.OnAddItem;
        UIEventListener.Get(btnRemoveItem).onClick = this.OnRemoveItem;
        if (btnRefresh != null)
            UIEventListener.Get(btnRefresh).onClick = this.OnRefreshWithTargetDataCount;
    }

    public void OnAddItem(GameObject obj)
    {
        for (int i = 0; i < perAddItemCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = (addCount++) * 100;
            scrollViewManager.itemDatas.Add(data);
        }
        scrollViewManager.RefreshLoopScrollView();
    }

    public void OnRemoveItem(GameObject obj)
    {
        // random remove item index.
        if (this.scrollViewManager.itemDatas.Count == 0)
        {
            Debuger.LogWarning("@ item count is zero.");
            return;
        }
        int index = Random.Range(0, scrollViewManager.itemDatas.Count);
        this.scrollViewManager.itemDatas.RemoveAt(index);
        this.scrollViewManager.RefreshLoopScrollView();
        Debuger.Log("@ remove item index : " + index);
    }

    // 根据当前的item数量刷新scrollview
    private int refreshCount = 1;
    public void OnRefreshWithTargetDataCount(GameObject obj)
    {
        this.newDataCount = int.Parse(inputNewDataCount.value);
        this.scrollViewManager.itemDatas.Clear();
        for (int i = 0; i < this.newDataCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = (i + 1) + (refreshCount * 100000);
            this.scrollViewManager.itemDatas.Add(data);
        }
        this.refreshCount++;
        this.scrollViewManager.RefreshLoopScrollView();

        Debuger.Log("@ refresh scroll with new data......");
        Debuger.Log("@ just reuse the item......");
    }
}

public class RankItemData
{
    public int rank;
}
