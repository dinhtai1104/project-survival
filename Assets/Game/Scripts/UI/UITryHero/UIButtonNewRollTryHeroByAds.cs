
using UnityEngine;

public class UIButtonNewRollTryHeroByAds : UIBaseButton
{
    public UITryHeroPanel TryHeroPanel;
    public override void Action()
    {
        Debug.Log("Show ADS To Reroll");
        TryHeroPanel.Reroll();
    }
}
