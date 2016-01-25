using UnityEngine;
using System.Collections;

public class LoopBaseItem : MonoBehaviour {

    private int curItemIndex;
    public int CurItemIndex
    {
        get { return this.curItemIndex; }
        set { this.curItemIndex = value; }
    }

    void Awake()
    {
        this.OnAwake();
    }

    // update the loop item data.
    public virtual void UpdateData(object updateData)
    {
    }

    protected virtual void OnAwake()
    {
    }
}
