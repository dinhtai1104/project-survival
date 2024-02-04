using Cysharp.Threading.Tasks;
using UI;

public static class AchievementQuestLocator
{
    public static void AchievementLocator(EAchievement type, UIQuestPanel panel)
    {

        switch (type)
        {
            case EAchievement.ClearDungeon:
                break;
            case EAchievement.OwnHeroPoison:
            case EAchievement.OwnHeroFrozen:
            case EAchievement.OwnHeroNinja:
            case EAchievement.OwnHeroJump:
            case EAchievement.OwnHeroRocket:
            case EAchievement.OwnHeroShinigami:
            case EAchievement.OwnHeroCowboy:
            case EAchievement.OwnHeroAngel:
            case EAchievement.OwnHeroEvil:
            case EAchievement.HeroLevel:
                if (DataManager.Save.ButtonFeature.IsUnlock(EFeature.Loadout))
                {
                    PanelManager.CreateAsync<UIHeroUpgradePanel>(AddressableName.UIHeroUpgradePanel)
                    .ContinueWith(t =>
                    {
                        t.Show();
                    }).Forget();
                }
                else
                {
                    PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.FeatureLocked")).Forget();
                    return;
                }
                break;
            case EAchievement.AccountLevel:
                break;
            case EAchievement.UpgradeEquipmentLevel:
                if (DataManager.Save.ButtonFeature.IsUnlock(EFeature.Loadout))
                {
                    PanelManager.CreateAsync<UIInventoryPanel>(AddressableName.UIInventoryPanel)
                    .ContinueWith(t =>
                    {
                        t.Show();
                    }).Forget();
                }
                else
                {
                    PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.FeatureLocked")).Forget();
                    return;
                }
                break;
            case EAchievement.WatchAds:
                break;
            case EAchievement.OpenChestSilver:
            case EAchievement.OpenChestGolden:
            case EAchievement.OpenChestHero:
                PanelManager.CreateAsync<UIShopPanel>(AddressableName.UIShopPanel)
                    .ContinueWith(t =>
                    {
                        t.Show();
                    }).Forget(); 
                break;
        }
        panel.Close();
    }
}