using UnityEngine;
using System.Collections;

/// <summary>
/// Focus target in scroll view
/// </summary>
public class UIFocusOnTarget
{
    public static void FocusOnTarget(UIWidget.Pivot pivot, UIScrollView scrollView, Transform target)
    {
        if (scrollView == null)
        {
            Debug.LogError("scrollView == null");
            return;
        }
        if (target == null)
        {
            Debug.LogError("target == null");
            return;
        }
        // corners
        // 1    2
        // 0    3

        Vector3[] corners = scrollView.panel.worldCorners;
        Vector3 toPosition = Vector3.zero;
        if (pivot == UIWidget.Pivot.Center)
            toPosition = (corners[0] + corners[2]) * 0.5f;
        else if (pivot == UIWidget.Pivot.Top)
            toPosition = (corners[1] + corners[2]) * 0.5f;
        else if (pivot == UIWidget.Pivot.Bottom)
            toPosition = (corners[0] + corners[3]) * 0.5f;
        else if (pivot == UIWidget.Pivot.TopLeft)
            toPosition = corners[1];
        else if (pivot == UIWidget.Pivot.TopRight)
            toPosition = corners[2];
        else if (pivot == UIWidget.Pivot.Left)
            toPosition = (corners[0] + corners[1]) * 0.5f;
        else if (pivot == UIWidget.Pivot.Right)
            toPosition = (corners[2] + corners[3]) * 0.5f;
        else if (pivot == UIWidget.Pivot.BottomLeft)
            toPosition = corners[0];
        else if (pivot == UIWidget.Pivot.BottomRight)
            toPosition = corners[3];
        MovePanelToTargetPos(scrollView, target, toPosition);
    }

    private static void MovePanelToTargetPos(UIScrollView mScrollView, Transform target, Vector3 panelPos)
    {
        Transform panelTrans = mScrollView.panel.cachedTransform;

        // Figure out the difference between the chosen child and the panel's center in local coordinates
        Vector3 cp = panelTrans.InverseTransformPoint(target.position);
        Vector3 cc = panelTrans.InverseTransformPoint(panelPos);
        Vector3 localOffset = cp - cc;

        // Offset shouldn't occur if blocked
        if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
        if (!mScrollView.canMoveVertically) localOffset.y = 0f;
        localOffset.z = 0f;

        // Spring the panel to this calculated position
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetPanelPosition(panelTrans, localOffset, mScrollView.panel);
        }
        else
#endif
        {
            // restrict with in bounds
            // SetPanelPosition(panelTrans, localOffset, mScrollView.panel);
            // mScrollView.RestrictWithinBounds(true);

            // spring panel without restrict with bounds
            SpringPanel.Begin(mScrollView.panel.cachedGameObject, panelTrans.localPosition - localOffset, 8f);
        }
    }

    private static void SetPanelPosition(Transform panelTrans, Vector3 localOffset, UIPanel targetPsanel)
    {
        panelTrans.localPosition = panelTrans.localPosition - localOffset;
        Vector4 co = targetPsanel.clipOffset;
        co.x += localOffset.x;
        co.y += localOffset.y;
        targetPsanel.clipOffset = co;
    }
}
