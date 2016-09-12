# LoopScorllViewForNGUI
powerful endless loop scroll view for Unity3d. 

-----Developed under unity3d version 5.x-----


## Introduction:

Endless loop scroll view for NGUI.

## Features:

1. easy to add base endless , loop scrollview in your project for showing up big data(particularly the rank list)

2. dynamic add new data to the endless scroll view.


3. dynamic delete data in list without resetting the scroll view(delete all the item recreate them all.)

## 介绍：

 Endless scrollview

## 功能：
 
 1. 方便添加到项目工程中
 
 2. 动态增加数据
 
 3. 动态删除数据，删除数据自动更新item，不用重置scrollview位置或者重新生成item

## Develop status

1. base endless and loop feature,both vertical and horizon(**done**)

2. dynamic add new data(**done**)

3. dynamic delete(**done**)

4. dynamic calculate min loop item cout(**done**) 

## How to Use(Example)
Very easy to use Just one method[RefreshLoopScrollView()]

### Show items
Fill the data list and call RefreshLoopScrollView();

<pre><code>
        scrollViewManager = this.GetComponent<LoopScrollViewManager>();
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

## **Update 2016.7.6**

1. 优化resetScrollView的方法，不直接的Destroy   (optimize the resetScrollView method)

2. 加入打印log的开关，场景中的[DebugSystem]物体上绑定 (add a Log switch look at [DebugSystem] GameObject in the scene)
