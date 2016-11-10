# LoopScorllViewForNGUI
## Easy endless loop scroll view for Unity3d. 

-----Developed under unity3d version 5.x-----


## Introduction:

Endless loop scroll view for NGUI.

## Features:

1. easy to add base endless , loop scrollview in your project for showing up big data(particularly the rank list)
2. dynamic add new data to the endless scroll view.
3. dynamic delete data in list without resetting the scroll view(delete all the item recreate them all.)
4. **......Some script for NGUI's scrollview extension......**

## Develop status

1. base endless and loop feature,both vertical and horizon(**done**)
2. dynamic add new data(**done**)
3. dynamic delete(**done**)
4. dynamic calculate min loop item cout(**done**) 

*****

## Start to Use (Example)

1. Very easy to use.**Just deal with the data list (add, delete , update)** and Call one method**[RefreshLoopScrollView()]** everything will be done.
2. Find **[DebugSystem]** to enable or disable the log

### Show items
Fill the data list and call RefreshLoopScrollView();

<pre><code>
        scrollViewManager = this.GetComponent< LoopScrollViewManager >();
        scrollViewManager.itemDatas.Clear();
        for (int i = 0; i < dataCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = i + 1;
            scrollViewManager.itemDatas.Add(data);
        }
        scrollViewManager.RefreshLoopScrollView();
</code></pre>

### Add new item
add new item data to list and call RefreshLoopScrollView();

<pre><code>
        for (int i = 0; i < perAddItemCount; i++)
        {
            RankItemData data = new RankItemData();
            data.rank = (addCount++) * 100;
            scrollViewManager.itemDatas.Add(data);
        }
        scrollViewManager.RefreshLoopScrollView();
</code></pre>

### Remove item
remove item data from data list call RefreshLoopScrollView();

<pre><code>
        if (this.scrollViewManager.itemDatas.Count == 0)
        {
            Debuger.LogWarning("@ item count is zero.");
            return;
        }
        int index = Random.Range(0, scrollViewManager.itemDatas.Count);
        this.scrollViewManager.itemDatas.RemoveAt(index);
        this.scrollViewManager.RefreshLoopScrollView();
</code></pre>

*****

## ScreenShot

![ScreenShot](https://github.com/tinyantstudio/LoopScorllViewForNGUI/blob/master/ScreenShot.png)
