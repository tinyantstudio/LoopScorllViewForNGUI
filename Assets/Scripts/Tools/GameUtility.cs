using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GameUtility : MonoBehaviour
{
    public static Dictionary<int, string> letter = new Dictionary<int, string>()
    {
        { 0, "A"},
        { 1, "B"},
        { 2, "C"},
        { 3, "D"},
        { 4, "E"},
        { 5, "F"},
        { 6, "G"},
        { 7, "H"},
        { 8, "I"},
        { 9, "J"},
    };

    public static string GetItemNameWithIndex ( int index )
    {
        StringBuilder sb = new StringBuilder();
        int offSet = index / 10;
        return sb.AppendFormat("{0}{1}", GameUtility.letter[offSet], index - offSet * 10).ToString();
    }

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
