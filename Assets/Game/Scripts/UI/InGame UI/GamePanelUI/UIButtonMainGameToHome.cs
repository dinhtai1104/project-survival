using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Logic.Memory;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIButtonMainGameToHome : UIBaseButton
{
    public override async void Action()
    {
        var notice = await PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/You will lose all data if you quit game!"));
        notice.SetConfirmCallback(OnConfirmQuitGame);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    private void OnConfirmQuitGame()
    {
        Debug.Log("QUIT 1");
        Time.timeScale = 1;


        var tryHero = DataManager.Save.User.TryHeroCurrent;
        if (tryHero != EHero.None)
        {
            //DataManager.Save.HotSaleHero.CanShowHeroSale = true;
            //DataManager.Save.HotSaleHero.HeroShowSale = tryHero;

            //// Unlock goi hero sale
            //DataManager.Save.HotSaleHero.Active(tryHero);
        }
        else
        {
            DataManager.Save.User.SetTryHero(EHero.None);
            DataManager.Save.HotSaleHero.CanShowHeroSale = false;
        }
        PlayerPrefs.SetInt("LastDungeon", GameController.Instance.GetSession().CurrentDungeon);
        if (GameController.Instance.GetSession().Mode == GameMode.DungeonEvent)
        {
            Architecture.Get<ShortTermMemoryService>().Remember(new MenuViewActionMemory()
            {
                address = AddressableName.UIDungeonEventPanel
            });
        }

        DataManager.Save.User.SetTryHero(EHero.None);
        GameSceneManager.Instance.PlayerData.HeroCurrent = DataManager.Save.User.HeroCurrent;
        DataManager.Save.ClearSession();
        GameController.Instance.ClearSession();
        Controller.Instance.LoadMenuScene().Forget();
    }
}