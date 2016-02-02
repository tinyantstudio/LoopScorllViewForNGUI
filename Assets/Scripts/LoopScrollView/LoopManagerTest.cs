using UnityEngine;
using System.Collections;

public class LoopManagerTest : MonoBehaviour
{
    public int dataCount = 100;

    public bool addItem = false;
    public int perAddItemCount = 1;

    public GameObject btnAddItem;
    public GameObject btnRemoveItem;

    private static int addCount = 1;

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
        scrollViewManager.ShowLoopScrollView();

        // set button listener.
        UIEventListener.Get(btnAddItem).onClick = this.OnAddItem;
        UIEventListener.Get(btnRemoveItem).onClick = this.OnRemoveItem;
    }

    public void OnAddItem(GameObject obj)
    {
        for (int i = 0; i < perAddItemCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = (addCount ++ ) * 100;
            scrollViewManager.itemDatas.Add(data);
        }
        scrollViewManager.RefreshLoopScrollView();
    }

    public void OnRemoveItem(GameObject obj)
    {
        // random remove item index.
        if (this.scrollViewManager.itemDatas.Count == 0)
        {
            Debug.LogWarning("@ item count is zero.");
            return;
        }
        int index = Random.Range(0, scrollViewManager.itemDatas.Count);
        this.scrollViewManager.itemDatas.RemoveAt(index);
        this.scrollViewManager.RefreshLoopScrollView();
        Debuger.Log("@ remove item index : " + index);
    }
}

public class RankItemData
{
    public int rank;
}
