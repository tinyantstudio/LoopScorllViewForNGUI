using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GameUtility : MonoBehaviour
{
    /// <summary>
    /// 添加子节点 Add new child to target.
    /// </summary>
    public static void AddChildToTarget ( Transform target, Transform child )
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
    public static void ChangeChildLayer ( Transform t, int layer )
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
