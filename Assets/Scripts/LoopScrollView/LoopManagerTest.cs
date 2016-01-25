using UnityEngine;
using System.Collections;

public class LoopManagerTest : MonoBehaviour
{
    public int dataCount = 100;

    public bool addItem = false;
    public int perAddItemCount = 1;
    public GameObject btnAddItem;

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

        UIEventListener.Get(btnAddItem).onClick = this.OnAddItem;
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
}

public class RankItemData
{
    public int rank;
}
