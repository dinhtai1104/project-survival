using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIButtonPlay : UIBaseButton
{
    [SerializeField] private TextMeshProUGUI energyCostTxt;
    private DungeonMapMenu dataLive;
    private ResourceData cost;
    public void Init()
    {
        energyCostTxt.text = "5"; // test
    }
    float triggerTime = 0;
    public override async void Action()
    {
        if (Time.time - triggerTime < 1f) return;
        triggerTime = Time.time;
        cost = new ResourceData { Resource = EResource.Energy, Value = 5 };

        var resource = DataManager.Save.Resources;
        if (resource.HasResource(cost))
        {
            dataLive = DataManager.Live.DungeonLive;
            var save = DataManager.Save.Dungeon;
            int dungeonPlay = save.CurrentDungeon;
            if (save.CanPlayDungeon(dataLive.CurrentDungeon))
            {
                dungeonPlay = dataLive.CurrentDungeon;
            }
            else
            {
                await PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.DungeonLocked"));
                return;
            }

            DataManager.Save.User.PlaySession();

            resource.DecreaseResource(cost);
            await PanelManager.Instance.GetPanel<UIMainMenuPanel>().HideByTransitions();
            DataManager.Save.ClearSession();
            var playerData = GameSceneManager.Instance.PlayerData;
            playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[DataManager.Save.User.HeroCurrent].heroStat, EStatSource.sourceHero);
            //dungeonPlay = dataLive.CurrentDungeon;
            await Game.Controller.Instance.StartLevel(GameMode.Normal, dungeonPlay);
        }
        else
        {
            PanelManager.CreateAsync<UIShopResourcePanel>(AddressableName.UIShopResourcePanel).ContinueWith(t =>
            {
                t.Show(EResource.Energy);
            }).Forget();
            //await PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, cost.Resource.GetLocalize()));
        }
    }
}
