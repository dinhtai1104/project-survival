using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;

public class UIChestHero : UIChestBase
{
    [SerializeField] private TextMeshProUGUI m_GetHeroInTxt;

    protected override void Setup()
    {
        base.Setup();
        m_GetHeroInTxt.text = I2Localize.GetLocalize("Common/Get_Hero_After", (50 - save.OpenCount % 50));

    }
    protected async override void Open()
    {
        base.Open();
        var rewardsRaw = new List<List<LootParams>>();
        var hero = EHero.NormalHero;
        bool isOpenedHero = false;
        var rewards = new List<LootParams>();
        for (int i = 0; i < NumberOpen; i++)
        {
            var rw = GetReward();
            if ((save.OpenCount + 1) % 50 == 0)
            {
                isOpenedHero = true;
                rw.RemoveAt(0);
                var randomHero = DataManager.Base.Hero.Dictionary.Keys.ToList().Random();
                var heroLootData = new HeroData { HeroType = randomHero };
                if (!userSave.IsUnlockHero(randomHero))
                {
                    rw.Add(new LootParams(ELootType.Hero, heroLootData));
                }
                else
                {
                    var loot = new LootParams(ELootType.HeroFragment, new ResourceData { Value = 20, Resource = EResource.NormalHero + (int)randomHero });
                    Debug.Log(loot);
                    rw.Add(loot);
                }
                hero = randomHero;
            }
            rewardsRaw.Add(rw);
            save.Open();

            foreach (var data in rw)
            {
                rewards.Add(data);
            }
        }
        rewards = rewards.Merge();
        var path = chestType == EChest.Hero10 ? AddressableName.UIChestRewardHero_10Panel : AddressableName.UIChestRewardHero_1Panel;

        if (chestType == EChest.Hero10)
        {
            PanelManager.CreateAsync<UIChestRewardBasePanel>(path).ContinueWith(async rewPanel =>
            {
                rewPanel.onClosed += () =>
                {
                    //PanelManager.ShowRewards(rewards);
                    if (isOpenedHero)
                    {
                        PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel).ContinueWith(rewPanel =>
                        {
                            rewPanel.Show(hero);
                            rewPanel.onClosed += () =>
                            {
                                PanelManager.ShowRewards(rewards);
                                CloudSave.Controller.Instance.ValidateAndSave().Forget();
                            };
                        }).Forget();
                    }
                    else
                    {
                        PanelManager.ShowRewards(rewards);
                        CloudSave.Controller.Instance.ValidateAndSave().Forget();
                    }
                };
                rewPanel.Show();
                rewPanel.SetRewardRaw(rewardsRaw);
                rewPanel.SetReward(base.chestType, rewards);
                await rewPanel.Stop();
            }).Forget();
        }
        else
        {
            PanelManager.CreateAsync<UIChestRewardBasePanel>(path).ContinueWith(async rewPanel =>
            {
                rewPanel.onClosed += () =>
                {
                    //PanelManager.ShowRewards(rewards);
                    if (isOpenedHero)
                    {
                        PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel).ContinueWith(rewPanel =>
                        {
                            rewPanel.Show(hero);
                            rewPanel.onClosed += () =>
                            {
                                PanelManager.ShowRewards(rewards);
                                CloudSave.Controller.Instance.ValidateAndSave().Forget();
                            };
                        }).Forget();
                    }
                    else
                    {
                        PanelManager.ShowRewards(rewards);
                        CloudSave.Controller.Instance.ValidateAndSave().Forget();
                    }
                };
                rewPanel.Show();
                rewPanel.SetRewardRaw(rewardsRaw);
                rewPanel.SetReward(base.chestType, rewards);
                await rewPanel.Stop();
            }).Forget();


            //if (isOpenedHero)
            //{
            //    PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel).ContinueWith(rewPanel =>
            //    {
            //        rewPanel.Show(hero);
            //        rewPanel.onClosed += () =>
            //        {
            //            PanelManager.ShowRewards(rewards);
            //        };
            //    }).Forget();
            //}
            //else
            //{
            //    PanelManager.CreateAsync<UIChestRewardBasePanel>(path).ContinueWith(async rewPanel =>
            //    {
            //        rewPanel.onClosed += () =>
            //        {
            //            PanelManager.ShowRewards(rewards);
            //        };
            //        rewPanel.Show();
            //        rewPanel.SetRewardRaw(rewardsRaw);
            //        rewPanel.SetReward(base.chestType, rewards);
            //        await rewPanel.Stop();
            //    }).Forget();
            //}
        }
    }
}