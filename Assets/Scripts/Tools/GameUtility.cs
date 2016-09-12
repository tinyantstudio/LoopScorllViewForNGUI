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
        { 6, "G" },
        { 7, "H"},
        { 8, "I"},
        { 9, "J"},
    };

    public static string GetItemNameWithIndex(int index)
    {
        StringBuilder sb = new StringBuilder();
        int offSet = index / 10;
        return sb.AppendFormat("{0}{1}", GameUtility.letter[offSet], index - offSet * 10).ToString();
    }
}
