using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using static UnityEngine.UI.Image;

public class UIHeroUpgradeLevelView : UIHeroUpgradeBase
{
    [SerializeField] private UIIconText costPanel;
    [SerializeField] private UIIconText stoneFragment;
    [SerializeField] private GameObject buttonUpgrade;
    private int CurrentLevel;
    private HeroLevelUpgradeEntity upgradeData;
    public override void Init(UIHeroUpgradePanel view, EHero eHero)
    {
        base.Init(view, eHero);
        Show();
    }

    private void Show()
    {
        costPanel.gameObject.SetActive(true);
        stoneFragment.gameObject.SetActive(true);

        var allUpgradeLevel = heroEntity.LevelUpgrades;
        foreach (var entity in allUpgradeLevel)
        {
            if (entity.Milestone)
            {
                var item = PoolManager.Instance.Spawn(affixPrefab, affixHolder).GetComponent<UIStatAffixHeroUpgradeLevel>();
                item.Set(entity);
                
                // add to pool for release
                allAffixes.Add(item.gameObject);
            }
        }

        CurrentLevel = userSave.GetHeroCurrent().Level;
        CurrentLevel = heroSave.Level;
        buttonUpgrade.SetActive(true);
        // Max level
        if (CurrentLevel >= allUpgradeLevel.Count)
        {
            buttonUpgrade.SetActive(false);
            costPanel.gameObject.SetActive(false);
            stoneFragment.gameObject.SetActive(false);
            return;
        }

        upgradeData = allUpgradeLevel[CurrentLevel];


        costPanel.Set(upgradeData.Cost);
        var costData = upgradeData.Cost;
        // Set Cost
        var enough = costData.Value <= resourcesSave.GetResource(costData.Resource);
        var color = enough ? "green" : "red";
        //var amountStr = $"{resourcesSave.GetResource(costData.Resource).TruncateValue()}/<color={color}>{costData.Value.TruncateValue()}</color>";
        var amountStr = $"{CSharpExtension.FormatRequire(resourcesSave.GetResource(costData.Resource), costData.Value)}";

        costPanel.SetAmountFormat(amountStr);

        // Set Stone
        stoneFragment.Set(upgradeData.Fragment);
        var stoneData = upgradeData.Fragment;
        enough = stoneData.Value <= resourcesSave.GetResource(stoneData.Resource);
        color = enough ? "green" : "red";
        //amountStr = $"{resourcesSave.GetResource(stoneData.Resource).TruncateValue()}/<color={color}>{stoneData.Value.TruncateValue()}</color>";
        amountStr = $"{CSharpExtension.FormatRequire(resourcesSave.GetResource(stoneData.Resource), stoneData.Value)}";

        stoneFragment.SetAmountFormat(amountStr);
    }

    public override void UpgradeOnClicked()
    {
        var allUpgradeLevel = heroEntity.LevelUpgrades;
        // Max level
        if (CurrentLevel >= allUpgradeLevel.Count)
        {
            costPanel.gameObject.SetActive(false);
            stoneFragment.gameObject.SetActive(false);
            //PanelManager.ShowNotice("MAX LEVEL").Forget();
            return;
        }


        if (!heroSave.IsUnlocked)
        {
            PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/You need to unlock this hero first")).Forget();
            return;
        }

        if (!resourcesSave.HasResource(upgradeData.Cost))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, upgradeData.Cost.Resource.GetLocalize())).Forget();
            Debug.Log("Not enough Cost: " + upgradeData.Cost);
            MenuGameScene.Instance.EnQueue(EFlashSale.Gold);

            return;
        }
        if (!resourcesSave.HasResource(upgradeData.Fragment))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, upgradeData.Fragment.Resource.GetLocalize())).Forget();
            Debug.Log("Not enough Fragment: " + upgradeData.Fragment);
            return;
        }
        Messenger.Broadcast(EventKey.HeroUpgradeLevelSuccess);

        upgradeEffect.Show().Forget();
        resourcesSave.DecreaseResources(upgradeData.Cost, upgradeData.Fragment);

        userSave.UpgradeLevelHero(heroSave.Type);
        playerData.RefreshStatHeroes();
        uiView.RefreshStat();

        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", upgradeData.Cost.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", upgradeData.Cost.Resource.ToString())
            .AddStringParam("source", "upgrade_level_hero")
            .AddStringParam("source_id", this.heroType.ToString())
            .AddDoubleParam("value", upgradeData.Cost.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(upgradeData.Cost.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(upgradeData.Cost.Resource))
            .Track();


        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", upgradeData.Fragment.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", upgradeData.Fragment.Resource.ToString())
            .AddStringParam("source", "upgrade_level_hero")
            .AddStringParam("source_id", this.heroType.ToString())
            .AddDoubleParam("value", upgradeData.Fragment.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(upgradeData.Fragment.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(upgradeData.Fragment.Resource))
            .Track();
    }
}