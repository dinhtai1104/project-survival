using Cysharp.Threading.Tasks;
using Spine;
using System;
using UI;
using UnityEngine;
public class UIHeroUpgradeStarView : UIHeroUpgradeBase
{
    [SerializeField] private UIIconText fragmentPanel;
    private HeroStarUpgradeEntity upgradeData;
    [SerializeField] private GameObject upgradeButton;
    public override void Init(UIHeroUpgradePanel view, EHero eHero)
    {
        base.Init(view, eHero);
        Show();
    }
    private void Show()
    {
        var allStarUpgrades = heroEntity.StarUpgrades;
        foreach (var entity in allStarUpgrades)
        {
            var item = PoolManager.Instance.Spawn(affixPrefab, affixHolder).GetComponent<UIStatAffixHeroUpgradeStar>();
            item.Set(entity);

            // add to pool for release
            allAffixes.Add(item.gameObject);
        }
        upgradeButton.SetActive(true);
        fragmentPanel.gameObject.SetActive(true);
        int currentStar = heroSave.Star;
        if (currentStar >= allStarUpgrades.Count)
        {
            upgradeButton.SetActive(false);
            fragmentPanel.gameObject.SetActive(false);
            return;
        }

        upgradeData = allStarUpgrades[currentStar];
        var cost = upgradeData.HeroFragment;

        fragmentPanel.Set(cost);
        var enough = cost.Value <= resourcesSave.GetResource(cost.Resource);
        var color = enough ? "green" : "red";
        //var amountStr = $"{resourcesSave.GetResource(cost.Resource).TruncateValue()}/<color={color}>{cost.Value.TruncateValue()}</color>";
        var amountStr = $"{CSharpExtension.FormatRequire(resourcesSave.GetResource(cost.Resource), cost.Value)}";

        fragmentPanel.SetAmountFormat(amountStr);
    }

    public override void UpgradeOnClicked()
    {
        if (!heroSave.IsUnlocked)
        {
            PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/You need to unlock this hero first")).Forget();
            return;
        }
        if (!resourcesSave.HasResource(upgradeData.HeroFragment))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, upgradeData.HeroFragment.Resource.GetLocalize())).Forget();
            Debug.Log("Not enough Cost: " + upgradeData.HeroFragment);
            return;
        }

        Messenger.Broadcast(EventKey.HeroUpgradeStarSuccess);

        upgradeEffect.Show().Forget();
        resourcesSave.DecreaseResource(upgradeData.HeroFragment);
        
        heroSave.UpgradeStar();
        playerData.RefreshStatHeroes();
        uiView.RefreshStat();

        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", upgradeData.HeroFragment.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", upgradeData.HeroFragment.Resource.ToString())
            .AddStringParam("source", "upgrade_hero")
            .AddStringParam("source_id", this.heroType.ToString())
            .AddDoubleParam("value", upgradeData.HeroFragment.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(upgradeData.HeroFragment.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(upgradeData.HeroFragment.Resource))
            .Track();
    }
}