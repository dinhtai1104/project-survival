using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Logic.DataModel.Noti;
using Assets.Game.Scripts.Logic.Memory;
using Assets.Game.Scripts.Subscription.Services;
using Cysharp.Threading.Tasks;
using Game;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;

public class MenuGameScene : MonoSingleton<MenuGameScene>
{
    public Panel menuPanel;
    private bool IsClickBackLastGame = false;
    private static bool IsShowedSubscription = false;

    public Queue<EFeature> FeatureUnlockQueue = new Queue<EFeature>();
    public Queue<EFlashSale> FlashSaleQueue = new Queue<EFlashSale>();

    private bool CanUpdate = false;

    private async UniTask Start()
    { 
        menuPanel = await PanelManager.CreateAsync("Panel/UIMainMenuPanel.prefab");
        if (!IsShowedSubscription)
        {
            if (DataManager.Save.ButtonFeature.IsUnlock(EFeature.Subscription) && Architecture.Get<SubscriptionService>().IsActiveAny() == false)
            {
                var sub = await PanelManager.CreateAsync(AddressableName.UISubscriptionPanel);
                var bo = false;
                sub.Show();
                sub.onClosed += () => bo = true;

                IsShowedSubscription = true;
                await UniTask.WaitUntil(() => bo);
            }
        }

        if (Architecture.Get<ShortTermMemoryService>().HasMemory<MenuViewActionMemory>())
        {
            var memory = Architecture.Get<ShortTermMemoryService>().RetrieveMemory<MenuViewActionMemory>();
            Architecture.Get<ShortTermMemoryService>().Forget<MenuViewActionMemory>();

            var ui = await memory.ActionMemory();
            bool finish = false;
            ui.Show();
            ui.onClosed += () => finish = true;
            ui.onClosed += () =>
            {
                menuPanel.Show();
            };

            await UniTask.WaitUntil(() => finish);
        }
        else
        {
            menuPanel.Show();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        if (DataManager.Save.User.NumberBackMenu >= 1)
        {
            if (DataManager.Save.FlashSale.GetSave(EFlashSale.StarterPack).Count == 0)
            {
                DataManager.Save.ButtonFeature.Unlock(EFeature.StarterPack);
                Architecture.Get<NotifiQueueService>().EnQueue(new NotifiQueueCheckFlashSaleStarterPack("CheckFlashSaleStarterPack"));
            }
        }

        CanUpdate = false;
        UpdateFeature().Forget();
        // Check old user cleared dungeon
        var dungeonSave = DataManager.Save.Dungeon;
        var dungeonTable = DataManager.Base.Dungeon.Dictionary.Count;
        int dungeon = dungeonSave.CurrentDungeon; 
        for (int i = 0; i < dungeon; i++)
        {
            dungeonSave.ShowAnimDungeon(i);
        }
        DataManager.Save.User.NumberBackMenu++;
        DataManager.Save.User.Save();
        DataManager.Save.FixData();
        ((UIMainMenuPanel)menuPanel).CheckButtonFeature().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        await CheckLastGameSave();


        if (!IsClickBackLastGame)
        {
            await Architecture.Get<NotifiQueueService>().FireNow(new NotifiQueueNewDungeon("New Dungeon"));
            Architecture.Get<NotifiQueueService>().EnQueue(new NotifiQueueCheckShowHeroSale("Check Show HeroSale"));
            Architecture.Get<NotifiQueueService>().EnQueue(new NotifiQueueCheckRewardDailySubscription("CheckRewardDailySubscription"));
            Architecture.Get<NotifiQueueService>().EnQueue(new NotifiQueueCheckPremiumReward("CheckPremiumReward"));
        }

        CanUpdate = true;

        ResourcesLoader.Instance.LoadAtlas("Equipment");
        ResourcesLoader.Instance.LoadAtlas("EquipmentType");
        ResourcesLoader.Instance.LoadAtlas("EquipmentRarity");
        ResourcesLoader.Instance.LoadAtlas("StarBuff");
    }
    private async UniTask UpdateFeature()
    {
        while (true)
        {
            if (!CanUpdate)
            {
                await UniTask.Yield();
                continue;
            }
            if (PanelManager.Instance.GetLast() == null)
            {
                await UniTask.Yield();
                continue;
            }
            // Check update for package
            await CheckFlashSale();
            try
            {
                if (PanelManager.Instance.GetLast().GetType() != typeof(UIMainMenuPanel))
                {
                    await UniTask.Yield();
                    continue;
                }
            }
            catch (Exception e)
            {

            }
            // Check update for detect button feature
            if (FeatureUnlockQueue.Count > 0)
            {
                var Feature = FeatureUnlockQueue.Dequeue();
                if (Feature == EFeature.StarterPack)
                {
                }
                else
                {
                    if (!DataManager.Save.ButtonFeature.IsUnlock(Feature))
                    {
                        var ui = await PanelManager.CreateAsync<UIUnlockFeaturePanel>(AddressableName.UIUnlockFeaturePanel);
                        ui.Show(Feature);
                        var close = false;
                        ui.onClosed += () => close = true;
                        await UniTask.WaitUntil(() => close);

                        if (Feature == EFeature.BattlePass)
                        {
                            var uiBP = await PanelManager.CreateAsync(AddressableName.UIGettingsBattlePassPanel);
                            uiBP.Show();
                            close = false;
                            uiBP.onClosed += () => close = true;
                            await UniTask.WaitUntil(() => close);
                        }

                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                        DataManager.Save.ButtonFeature.Unlock(Feature);
                    }
                    continue;
                }
            }
            await Architecture.Get<NotifiQueueService>().Update();
            await UniTask.Yield();
        }
    }

    private async UniTask CheckFlashSale()
    {
        if (FlashSaleQueue.Count > 0)
        {
            await UniTask.Delay(200);
            var FlashSale = FlashSaleQueue.Dequeue();
            DataManager.Save.FlashSale.Show(FlashSale);
            var ui = await PanelManager.CreateAsync<UIFlashSalePanel>(AddressableName.UIFlashSalePanel.AddParams(FlashSale));
            ui.Show(FlashSale);
            var close = false;
            ui.onClosed += () => close = true;
            await UniTask.WaitUntil(() => close);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
        await UniTask.Yield();
    }

    public void EnQueue(EFlashSale sale)
    {
        if (FlashSaleQueue.Contains(sale)) return;
        var save = DataManager.Save.FlashSale.GetSave(sale);
        if (save.IsEnd)
        {
            save.Deactive();
        }
        FlashSaleQueue.Enqueue(sale);
    }
    public void EnQueue(EFeature feature)
    {
        FeatureUnlockQueue.Enqueue(feature);
    }

    #region lastgame
    private async UniTask CheckLastGameSave()
    {
        var lastSave = DataManager.Save.DungeonSession;
        if (lastSave.Mode == GameMode.Tutorial)
        {
            DataManager.Save.User.SetTryHero(EHero.None);
            DataManager.Save.ClearSession();
            return;
        }
        var lastSaveEvent = DataManager.Save.DungeonEventSession;
        bool wait = false;

        if (lastSaveEvent.IsPlaying && !lastSaveEvent.IsAskFirstTime)
        {
            lastSaveEvent.IsAskFirstTime = true;
            var ui = await PanelManager.ShowNotice(I2Localize.GetLocalize("Common/Back_To_Game_Last_Save"));
            ui.SetCancelCallback(CancelBackToGameEvent);
            ui.SetConfirmCallback(BackToLastGameEvent);
            lastSaveEvent.Save();
            ui.onClosed += () => wait = true;
            await UniTask.WaitUntil(() => wait);
        }

        if (lastSave.IsPlaying && !lastSave.IsAskFirstTime)
        {
            lastSave.IsAskFirstTime = true;
            var ui = await PanelManager.ShowNotice(I2Localize.GetLocalize("Common/Back_To_Game_Last_Save"));
            ui.SetCancelCallback(CancelBackToGame);
            ui.SetConfirmCallback(BackToLastGame);
            lastSave.Save();
            ui.onClosed += () => wait = true;
            await UniTask.WaitUntil(() => wait);
        }
    }

    private async void BackToLastGameEvent()
    {
        IsClickBackLastGame = true;
        await PanelManager.Instance.GetPanel<UIMainMenuPanel>().HideByTransitions();
        Game.Controller.Instance.StartLevel(GameMode.DungeonEvent, DataManager.Save.DungeonEventSession.CurrentDungeon, DataManager.Save.DungeonEventSession.Type).Forget();

        FirebaseAnalysticController.Tracker.NewEvent("button_click")
            .AddStringParam("category", "notice")
            .AddStringParam("name", "back_to_last_game")
            .Track();
    }

    private void CancelBackToGameEvent()
    {
        DataManager.Save.User.SetTryHero(EHero.None);
        DataManager.Save.DungeonEventSession.Clear();
    }

    private void CancelBackToGame()
    {
        DataManager.Save.User.SetTryHero(EHero.None);
        DataManager.Save.ClearSession();
    }
    private async void BackToLastGame()
    {
        IsClickBackLastGame = true;
        await PanelManager.Instance.GetPanel<UIMainMenuPanel>().HideByTransitions();
        Game.Controller.Instance.StartLevel(GameMode.Normal, DataManager.Save.DungeonSession.CurrentDungeon).Forget();
    }
    #endregion


}