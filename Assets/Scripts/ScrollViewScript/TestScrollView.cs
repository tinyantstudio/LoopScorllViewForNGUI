using UnityEngine;
using System.Collections;

public class TestScrollView : MonoBehaviour
{
    public UIWidget.Pivot pivot;
    public GameObject target;
    public UIScrollView scrollView;
    public GameObject btnClickFocus;

    void Start()
    {
        UIEventListener.Get(btnClickFocus).onClick = OnClickFocus;
    }
    private void OnClickFocus(GameObject obj)
    {
        UIFocusOnTarget.FocusOnTarget(this.pivot, this.scrollView, this.target.transform);
        Debug.Log(string.Format("## Focus on target {0} {1} ##", this.target.name, this.pivot.ToString()));
    }
}
