using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Handler;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTrialPoint : MonoBehaviour
{
    public MMF_Player openFb,selectFb;

    ActorBase newPlayer;

    IAssetLoader assetLoader = new AddressableAssetLoader();
    EHero recommendedHero;
    EHero currentHero;
    EquipmentHandler equipmentHandlerTrialHero;

    public void Prepare()
    {
        var dataSave = DataManager.Save.TryHero;
        currentHero = DataManager.Save.User.HeroCurrent;
        recommendedHero = dataSave.HeroRecommend;
        if (recommendedHero != EHero.None)
        {
            PrepareHero(recommendedHero);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
    async UniTask PrepareHero(EHero hero)
    {
        newPlayer = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(string.Format(AddressableName.Player, (int)(hero)), Game.Pool.EPool.Pernament)).GetComponent<ActorBase>();

        var mainWeapon = GameSceneManager.Instance.PlayerData.EquipmentHandler.GetEquipment(EEquipment.MainWeapon);

        WeaponBase playerWeapon = await assetLoader.LoadAsync<WeaponBase>("Weapon_" +  mainWeapon.Id).Task;
        var weaponData = new WeaponData
        {
            Weapon = playerWeapon,
            Item = mainWeapon
        };
        newPlayer.WeaponHandler.LoadWeapon(playerWeapon, weaponData);
        var stats = GetStatPlayer();

        // Add DPS change listener

        //Logger.Log("1. SETUP PLAYER " + newPlayer.gameObject.name+" "+stats.GetStat(StatKey.JumpCount).BaseValue+" " + stats.GetStat(StatKey.JumpCount).Value);
        newPlayer.SetActive(false);

        await ((Character)newPlayer).SetUp(stats);

        newPlayer.AnimationHandler.SetSkin("-1");


        newPlayer.SetPosition(transform.position);
        newPlayer.Input.SetActive(false);


        await UniTask.Yield();

        ((Character)newPlayer).SetLookDirection(0, 0);
        newPlayer.SetFacing(-1);
        newPlayer.PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);
        newPlayer.SetActive(true);

    }

    private IStatGroup GetStatPlayer()
    {
        // create new stat for trial hero
        var heroCurrent = DataManager.Save.User.GetHero(currentHero);
        var stats = HeroFactory.Instance.GetHeroStat(recommendedHero, heroCurrent.Level, heroCurrent.Star);
        stats = HeroFactory.Instance.ApplyEquipment(stats, GameSceneManager.Instance.PlayerData.EquipmentHandler, out equipmentHandlerTrialHero);

        stats.CalculateStats();
        return stats;
    }


    //player trigger trial ui
    public void Trigger()
    {
        GameUIPanel.Instance.inputController.SetActive(false);
        openFb.PlayFeedbacks();
    }

    public void ShowUI()
    {
        UI.PanelManager.CreateAsync<UITryHeroPanel>(AddressableName.UITryHeroPanel).ContinueWith(panel =>
        {
            panel.SetUp(recommendedHero);
            panel.onPickHero = (hero)=> {
                panel.onPickHero = null;

                // pick trial hero
                if (hero == recommendedHero)
                {
                    SetHero(newPlayer);
                    selectFb?.PlayFeedbacks();
                    GameController.Instance.GetSession().SetTriedHero(true);
                }
                else
                {
                    GameController.Instance.GetSession().SetTriedHero(false);
                    newPlayer.SetActive(false);
                }
                DataManager.Save.TryHero.SetTried(hero);
                GameUIPanel.Instance.inputController.SetActive(true);
                panel.Close();
                Hide();
            };
        }).Forget();
    }

    //set current player to new trial hero
    async void SetHero(ActorBase player)
    {
        ActorBase current = Game.Controller.Instance.gameController.GetMainActor();

        current.PassiveEngine.RemovePassives();
        //deactive current player &healthbar
        if (HealthBarHandler.Instance.Get(current) != null)
            HealthBarHandler.Instance.Get(current).SetActive(false);
        current.SetActive(false);

        await UniTask.Delay(250);
        //set new player;
        Game.Controller.Instance.gameController.SetMainActor(player);

        CalculateDPS(player.Stats);

        CameraController.Instance.Follow(player.GetMidTransform(), Vector2.zero, true);
        Messenger.Broadcast(EventKey.ActorSpawn, (ActorBase)player, true, -1);

        equipmentHandlerTrialHero.EquipPassive(player);
       


        player.StartBehaviours();
        player.Input.SetActive(true);
        DataManager.Save.HotSaleHero.HeroShowSale = recommendedHero;
        DataManager.Save.HotSaleHero.CanShowHeroSale = true;
        DataManager.Save.HotSaleHero.Active(recommendedHero);
        DataManager.Save.HotSaleHero.Save();

        //Change Drone



        Messenger.Broadcast(EventKey.ChangePlayer, (ActorBase)player);
    }
    #region Add DPS Event
    private void CalculateDPS(IStatGroup stats)
    {

    }
    private void RemoveCalculateDPSListener(IStatGroup stats)
    {
    }
    private void OnRecalculateNewDPS(float value)
    {
    }
    #endregion

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
