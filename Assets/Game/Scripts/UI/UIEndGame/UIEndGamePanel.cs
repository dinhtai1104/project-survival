using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Logic.Memory;
using Assets.Game.Scripts.Subscription.Services;
using com.debug.log;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Foundation.Game.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UI;
using UnityEngine;

public class UIEndGamePanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI titleTxt;

    [SerializeField] private TextMeshProUGUI zoneTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI bestWaveTxt;
    [SerializeField] private UILootCollectionView lootView;
    [SerializeField] private RectTransform parentReward;
    private DungeonSave zoneSave;
    private DungeonSessionSave dungeonSession;
    private ResourcesSave resourceSave;
    EBattleResult result;
    private EHero heroUsed;
#if DEVELOPMENT
    [Serializable]
    public class TestData
    {
        public List<string> lootparams;

        public TestData(List<string> lootparams)
        {
            this.lootparams = lootparams;
        }
    }
    public TestData rewardDevelopmentPost;
    public List<LootParams> listLootDevelopementPost;
#endif

    public List<LootParams> itemRewards;

    public GameObject buttonGetExtraGold, buttonSubscriptionNoAd;

    public override void PostInit()
    {
        zoneSave = GameController.Instance.GetDungeonSave();
        dungeonSession = GameController.Instance.GetSession();
        resourceSave = DataManager.Save.Resources;
    }
    public void FinishEndGame()
    {
        if (result == EBattleResult.Win && PlayerPrefs.GetInt("Rate",0)==0)
        {
            ReviewHandler.Request().ContinueWith(result => 
            {
                if (result)
                {
                    PlayerPrefs.SetInt("Rate", 1);
                }
            }).Forget();
        }
    }
    public void Show(EBattleResult result)
    {
        this.result = result;
        base.Show();
        titleTxt.text = I2Localize.GetLocalize($"Common/Title_{result}");
        zoneTxt.SetText(I2Localize.GetLocalize($"Dungeon/Dungeon_{dungeonSession.CurrentDungeon + 1}"));

        if (dungeonSession.Mode == GameMode.DungeonEvent)
        {
            zoneTxt.text = I2Localize.GetLocalize($"Common/DungeonEvent_Title_{(dungeonSession as DungeonEventSessionSave).Type}");
        }

        waveTxt.SetText($"<color=green>{dungeonSession.CurrentStage + (result == EBattleResult.Win ? 1 : 0)}</color>/{dungeonSession.MemoryMap.roomId.Count}");
        bestWaveTxt.text = "";

        if (result == EBattleResult.Lose)
        {
            var usr = DataManager.Save.User;
            if (usr.IsFirstDie == false)
            {
                usr.IsFirstDie = true;
                usr.Save();
            }
        }

        if(zoneSave.CurrentDungeon == dungeonSession.CurrentDungeon)
        {
            if (zoneSave.BestStage < dungeonSession.CurrentStage)
            {
                bestWaveTxt.SetText(I2Localize.GetLocalize("Best Cleaned: ") + $"<color=green>{dungeonSession.CurrentStage + 1}</color>" + "/" + dungeonSession.MemoryMap.roomId.Count);
            }
        }

        var reward = new List<LootParams>();
        itemRewards = new List<LootParams>();  
        foreach (var data in dungeonSession.lootData)
        {
            reward.Add(new LootParams(data));
            itemRewards.Add(new LootParams(data));
        }
        itemRewards = reward.Refactor();

        var mainActor = GameController.Instance.GetMainActor();
        var subscription = Architecture.Get<SubscriptionService>();
        foreach (var data in itemRewards)
        {
            if (data.Type == ELootType.Resource)
            {
                var dataRe = data.Data as ResourceData;
                if (dataRe != null)
                {
                    if (dataRe.Resource == EResource.Gold)
                    {
                        if (mainActor != null)
                        {
                            Debug.Log("Apply Coin Mul\n Subscription: " + subscription.GetExtraGoldDungeon() + "\nCoin Mul Player: " + mainActor.Stats.GetValue(StatKey.CoinMul));
                            dataRe.Multiply((subscription.GetExtraGoldDungeon() - 1) + mainActor.Stats.GetValue(StatKey.CoinMul));
                        }
                    }
                }
            }
            var exp = data.Data as ExpData;
            if (exp != null)
            {
                exp.Multiply(subscription.GetExtraExpDungeon());
            }

        }

        //var reward = dungeonSession.lootData;
        reward = reward.Refactor();
        reward.ForEach(e => e.Data.Loot());

        DataManager.Save.User.Play();

#if DEVELOPMENT
        listLootDevelopementPost = reward;
        rewardDevelopmentPost = new TestData(new List<string>(dungeonSession.lootData));
#endif
        if (dungeonSession.Mode == GameMode.Normal)
        {
            int currentDungeon = dungeonSession.CurrentDungeon + 1;
            if (dungeonSession.CurrentStage >= GameController.Instance.GetDungeonEntity().Stages.Count - 1)
            {
                if (result == EBattleResult.Win)
                {
                    Architecture.Get<AchievementService>().SetProgress(EAchievement.ClearDungeon, currentDungeon);
                }
            }
        }

        if (dungeonSession.CurrentStage >= GameController.Instance.GetDungeonEntity().Stages.Count - 1)
        {
            int currentDungeon = dungeonSession.CurrentDungeon;
            if (zoneSave.DungeonCleared.Count == 0 || currentDungeon > zoneSave.DungeonCleared.Last())
            {
                if (result == EBattleResult.Win)
                {
                    zoneSave.ClearDungeon(dungeonSession.CurrentDungeon);
                    zoneSave.CurrentDungeon++;
                    zoneSave.BestStage = 0;
                    zoneSave.Save();
                }
            }
        }

        var tryHero = DataManager.Save.User.TryHeroCurrent;
        if (tryHero != EHero.None)
        {
            heroUsed = tryHero;
            //DataManager.Save.HotSaleHero.CanShowHeroSale = true;
            //DataManager.Save.HotSaleHero.HeroShowSale = tryHero;

            //// Unlock goi hero sale
            //DataManager.Save.HotSaleHero.Active(tryHero);
        }
        else
        {
            heroUsed = DataManager.Save.User.Hero;
            DataManager.Save.User.SetTryHero(EHero.None);
            DataManager.Save.HotSaleHero.CanShowHeroSale = false;
        }

        OnUpdateUI();

        lootView.Show(new LootCollectionData(itemRewards)).Forget();
#if DEVELOPMENT
        SendMessageReward();
#endif
        PlayerPrefs.SetInt("LastDungeon", dungeonSession.CurrentDungeon);

        if (GameController.Instance.GetSession().Mode == GameMode.DungeonEvent)
        {
            Architecture.Get<ShortTermMemoryService>().Remember(new MenuViewActionMemory()
            {
                address = AddressableName.UIDungeonEventPanel
            });
        }

        buttonGetExtraGold.SetActive(itemRewards.Find(t=>t.Type == ELootType.Resource && (t.Data as ResourceData).Resource == EResource.Gold) != null);

        Track();
        dungeonSession.Clear();
    }

    private void Track()
    {
        var actor = GameController.Instance.GetMainActor();
        int remainhp = 1000;
        if (actor != null)
        {
            remainhp = (int)actor.HealthHandler.GetHealth();
        }
    }

    private async UniTask ShowReward(List<LootParams> data)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        await lootView.Show(new LootCollectionData(data));
    }

#if DEVELOPMENT
    public void SendMessageReward()
    {
        var Discord = new DiscordAPI();
        StringBuilder str = new StringBuilder(JsonUtility.ToJson(rewardDevelopmentPost));

        str.Append("\n");
        var listPar = new List<LootParams>();
        foreach (var loot in listLootDevelopementPost)
        {
            var toString = loot.ToString();
            str.Append(toString);
            str.Append("\n");
        }

        StartCoroutine(Discord.PostMessage(str.ToString()));
        Debug.Log(str);
        listLootDevelopementPost = listLootDevelopementPost.Refactor();
       // lootView.Show(new LootCollectionData(listLootDevelopementPost)).Forget();
    }
    
#endif

    private void OnUpdateUI()
    {
        var subscr = Architecture.Get<SubscriptionService>();
        buttonSubscriptionNoAd.SetActive(!subscr.IsFreeRewardAd());
    }

    public void NoAdsSubscriptionOnClicked()
    {
        PanelManager.CreateAsync<UISubscriptionPanel>(AddressableName.UISubscriptionPanel)
            .ContinueWith(t =>
            {
                t.Show();
                t.onClosed += OnUpdateUI;
            }).Forget();
    }
    public void GetExtraGoldOnClicked()
    {
        Architecture.Get<AdService>().ShowRewardedAd("ui_end_game_get_extra", OnFinishAdGetExtraGold, "ui_end_game_get_extra");
    }

    private void OnFinishAdGetExtraGold(bool result)
    {
        if (result == false) return;

        var gold = itemRewards.Find(t => t.Data is ResourceData && (t.Data as ResourceData).Resource == EResource.Gold);
        if (gold != null)
        {
            (gold.Data as ResourceData).Multiply(1.5f);
        }
        lootView.Clear(true);
        lootView.Show(new LootCollectionData(itemRewards)).Forget();
        OnUpdateUI();
        buttonGetExtraGold.SetActive(false);
    }
}
