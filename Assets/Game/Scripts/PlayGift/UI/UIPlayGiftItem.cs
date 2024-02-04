using Cysharp.Threading.Tasks;
using Mosframe;
using NSubstitute;
using Sirenix.OdinInspector;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayGiftItem : UIBehaviour, IDynamicScrollViewItem
{
    [ReadOnly] public int index = -1;
    [SerializeField] private UIPlayGiftPanel panel;
    [SerializeField] private UIInventorySlot slot;
    [SerializeField] private TextMeshProUGUI playTitleTxt;
    [SerializeField] private GameObject buttonClaim, buttonClaimed, buttonKeepPlay, symbolArrow;
    private Sprite sprite;

    private UIGeneralBaseIcon rewardLoadAsync;
    private int INDEX => index + 1;
    private PlayGiftEntity entity;
    private PlayGiftSaves saves;

    private CancellationTokenSource cts;
    public int getIndex()
    {
        return index;
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
        if (index < DataManager.Base.PlayGift.Dictionary.Count - 1)
        {
            symbolArrow.SetActive(true);
        }
        else
        {
            symbolArrow.SetActive(false);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cts == null)
        {
            cts = new CancellationTokenSource();
        }
        else
        {
            cts.Cancel();
        }
        cts = new CancellationTokenSource();
        saves = DataManager.Save.PlayGift;
        entity = DataManager.Base.PlayGift.Get(INDEX);

        slot.Clear();

        // Clear all 
        var icons = slot.GetComponentsInChildren<UIGeneralBaseIcon>();
        foreach (var icon in icons)
        {
            icon.Clear();
            PoolManager.Instance.Despawn(icon.gameObject);
        }


        buttonClaim.SetActive(false);
        buttonClaimed.SetActive(false);
        buttonKeepPlay.SetActive(false);

        if (entity == null) return;
        playTitleTxt.text = I2Localize.GetLocalize("Common/Title_PlayGift_PlayTime", entity.Id);
        var reward = entity.Reward;
        var path = AddressableName.UILootItemPath.AddParams(reward.Type);
        if (reward.Type == ELootType.Equipment)
        {
            path = AddressableName.UIGeneralEquipmentItem;
        }
        UIHelper.GetUILootIcon(path, reward.Data, slot.transform)
            .AttachExternalCancellation(cts.Token)
            .ContinueWith(t =>
            {
                slot.ActiveRarity(true);
                if (!(reward.Data is EquipmentData))
                {
                    slot.ActiveRarity(false);
                }
                slot.SetItem(t);
                sprite = t.Sprite;
            })
            .Forget();

        if (saves.IsClaimedPlay(INDEX))
        {
            buttonClaimed.SetActive(true);
        }
        else if (saves.CanClaimedPlay(INDEX))
        {
            buttonClaim.SetActive(true);
        }
        else if (!saves.CanClaimedPlay(INDEX))
        {
            buttonKeepPlay.SetActive(true);
        }
    }

    public void KeepPlayOnClicked()
    {
        panel.Close();
    }

    public void ClaimOnClicked()
    {
        panel.ClaimReward(INDEX, sprite);
    }
}
