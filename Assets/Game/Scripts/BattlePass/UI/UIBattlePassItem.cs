using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using Mosframe;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class UIBattlePassItem : UIEntityBehaviour, IDynamicScrollViewItem
    {
        [SerializeField] private UIBattlePassContainer container;
        public int index;
        [SerializeField] private TextMeshProUGUI levelTextMesh;
        [SerializeField] private UIInventorySlot premiumSlot;
        [SerializeField] private UIInventorySlot freeSlot;

        [SerializeField] private Image lockStatus_PremiumImg;
        [SerializeField] private Image lockStatus_FreeImg;

        private BattlePassEntity entity;
        private BattlePassSave save;
        private CancellationTokenSource cancellation;
        private BattlePassService service;

        private void Awake()
        {
            service = Architecture.Get<BattlePassService>();
        }

        public int getIndex()
        {
            return index;
        }

        public void onUpdateItem(int index)
        {
            if (cancellation == null)
            {
                cancellation = new CancellationTokenSource();
            }
            else
            {
                cancellation.Cancel();
            }
            cancellation = new CancellationTokenSource();
            this.index = index;
            UpdateUI();
        }

        private void UpdateUI()
        {
            entity = DataManager.Base.BattlePass.Get(index + 1);
            save = service.GetBattlePass(index + 1);
            lockStatus_PremiumImg.enabled = true;
            lockStatus_FreeImg.enabled = true;

            if (service.Level < entity.Level)
            {
                lockStatus_FreeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Lock");
                lockStatus_PremiumImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Lock");

                if (!service.IsPremium)
                {
                    lockStatus_PremiumImg.enabled = true;
                    lockStatus_PremiumImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Lock");
                }
                else
                {
                    lockStatus_PremiumImg.enabled = false;
                }
            }
            else
            {
                if (save == null)
                {
                    lockStatus_PremiumImg.enabled = false;
                    lockStatus_FreeImg.enabled = false;

                    if (!service.IsPremium)
                    {
                        lockStatus_PremiumImg.enabled = true;
                        lockStatus_PremiumImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Lock");
                    }
                    else
                    {
                        lockStatus_PremiumImg.enabled = false;
                    }
                }
                else
                {
                    lockStatus_PremiumImg.enabled = false;
                    lockStatus_FreeImg.enabled = false;
                    if (save.FreeClaimed)
                    {
                        lockStatus_FreeImg.enabled = true;
                        lockStatus_FreeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Claimed");
                    }
                   
                    if (!service.IsPremium)
                    {
                        lockStatus_PremiumImg.enabled = true;
                        lockStatus_PremiumImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Lock");
                    }
                    else
                    {
                        lockStatus_PremiumImg.enabled = false;
                    }

                    if (save.PremiumClaimed)
                    {
                        lockStatus_PremiumImg.enabled = true;
                        lockStatus_PremiumImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.BattlePass, "Claimed");
                    }
                }
            }

            lockStatus_FreeImg.SetNativeSize();
            lockStatus_PremiumImg.SetNativeSize();

            premiumSlot.ActiveRarity(true);
            freeSlot.ActiveRarity(true);

            premiumSlot.Clear();
            freeSlot.Clear();

            premiumSlot.GetComponent<Image>().enabled = true;
            freeSlot.GetComponent<Image>().enabled = true;

            levelTextMesh.text = entity.Level.ToString();
            var premiumRw = entity.PremiumReward;
            var freeRw = entity.FreeReward;

            var pathPremiumIcon = AddressableName.UIGeneralEquipmentItem;
            if (premiumRw.Type != ELootType.Equipment)
            {
                pathPremiumIcon = AddressableName.UILootItemPath.AddParams(premiumRw.Type);
            }
            UIHelper.GetUILootIcon(pathPremiumIcon, premiumRw.Data, premiumSlot.transform)
                .AttachExternalCancellation(cancellation.Token)
                .ContinueWith(icon=>
                {
                    if (icon)
                    {
                        premiumSlot.SetItem(icon);
                        if (premiumRw.Type != ELootType.Equipment)
                        {
                            premiumSlot.ActiveRarity(false);
                        }
                    }
                }).Forget();

            var pathFreeIcon = AddressableName.UIGeneralEquipmentItem;
            if (freeRw.Type != ELootType.Equipment)
            {
                pathFreeIcon = AddressableName.UILootItemPath.AddParams(freeRw.Type);
            }
            UIHelper.GetUILootIcon(pathFreeIcon, freeRw.Data, freeSlot.transform)
                .AttachExternalCancellation(cancellation.Token)
                .ContinueWith(icon =>
                {
                    if (icon)
                    {
                        freeSlot.SetItem(icon);
                        if (freeRw.Type != ELootType.Equipment)
                        {
                            freeSlot.ActiveRarity(false);
                        }
                    }
                }).Forget();
        }

        public void FreeOnClicked()
        {
            if (entity.Level > service.Level)
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.NeedPlayToUnlockThis", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            if (service.IsClaimed(entity.Level, EBattlePass.Free))
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.YouClaimed", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            if (index > 0 && !service.IsClaimed(entity.Level - 1, EBattlePass.Free))
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.YouNeedClaimBefore", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            service.ClaimBattlePass(index + 1, EBattlePass.Free);
            PanelManager.ShowRewards(entity.FreeReward).Forget();
            UpdateUI();
            container.UpdateProgress();
        }
        public void PremiumOnClicked()
        {
            if (!service.IsPremium)
            {
                PanelManager.CreateAsync(AddressableName.UIGettingsBattlePassPanel)
                    .ContinueWith(t =>
                    {
                        t.Show();
                    }).Forget();
                return;
            }
            if (entity.Level > service.Level)
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.NeedPlayToUnlockThis", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            if (service.IsClaimed(entity.Level, EBattlePass.Premium))
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.YouClaimed", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            if (index > 0 && !service.IsClaimed(entity.Level - 1, EBattlePass.Premium))
            {
                PanelManager.ShowNotice("Notice/Notice.BattlePass.YouNeedClaimBefore", true, noticeType: ENotice.OnlyYes).Forget();
                return;
            }
            service.ClaimBattlePass(index + 1, EBattlePass.Premium);
            PanelManager.ShowRewards(entity.PremiumReward).Forget();
            UpdateUI();
            container.UpdateProgress();
        }
    }
}