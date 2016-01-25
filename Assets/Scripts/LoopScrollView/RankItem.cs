using UnityEngine;
using System.Collections;

public class RankItem : LoopBaseItem {
    private UILabel lbRankNo;

    protected override void OnAwake()
    {
        this.lbRankNo = this.GetComponentInChildren<UILabel>();
    }
    public override void UpdateData(object updateData)
    {
        if (updateData is RankItemData)
        {
            RankItemData data = updateData as RankItemData;
            this.lbRankNo.text = data.rank.ToString();
        }
    }
}
