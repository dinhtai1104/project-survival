using Cysharp.Threading.Tasks;
using UI;

public static class DailyQuestLocator
{
    public static void MissionLocator(EMissionDaily type, UIQuestPanel panel)
    {

        switch (type)
        {
            case EMissionDaily.PlayDungeon:
                break;
            case EMissionDaily.Login:
                break;
            case EMissionDaily.PlayDungeonGold:
            case EMissionDaily.PlayDungeonScroll:
            case EMissionDaily.PlayDungeonStone:
                if (DataManager.Save.ButtonFeature.IsUnlock(EFeature.DungeonEvent))
                {
                    PanelManager.CreateAsync<UIDungeonEventPanel>(AddressableName.UIDungeonEventPanel).ContinueWith(t =>
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
            case EMissionDaily.WatchAds:
                break;
            case EMissionDaily.UpgradeEquipment:
                if (DataManager.Save.ButtonFeature.IsUnlock(EFeature.Loadout))
                {
                    PanelManager.CreateAsync<UIInventoryPanel>(AddressableName.UIInventoryPanel).ContinueWith(t =>
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
            case EMissionDaily.OpenChestSilver:
            case EMissionDaily.OpenChestGolden:
            case EMissionDaily.OpenChestHero:
                PanelManager.CreateAsync<UIShopPanel>(AddressableName.UIShopPanel).ContinueWith(t =>
                {
                    t.Show();
                }).Forget();
                break;
        }
        panel.Close();
    }
}