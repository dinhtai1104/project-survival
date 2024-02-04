using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIOfferBaseItem : UIShopOfferItemBase
{
    protected OfferBaseEntity entity;
    protected OfferItemSave save;
    protected ResourcesSave resources;

    [SerializeField] protected Button button;
    [SerializeField] protected GameObject m_FirstTime_GO;
    [SerializeField] protected TextMeshProUGUI m_ValueTxt;
    [SerializeField] protected TextMeshProUGUI m_ValueFirstTimeTxt;
    [SerializeField] protected Image m_Icon;

    protected LootParams LootReward;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClickOffer);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickOffer);
    }

    protected abstract void OnClickOffer();

    public void SetData(OfferBaseEntity e, OfferItemSave save)
    {
        this.entity = e;
        this.save = save;
        this.resources = DataManager.Save.Resources;
        Setup();
    }

    protected virtual void Setup()
    {
        m_FirstTime_GO.SetActive(!save.IsBoughtFirstTime);
        m_ValueFirstTimeTxt.SetText("+" + entity.ValueFirstTime.TruncateValue());
        if (save.IsBoughtFirstTime)
        {
            m_ValueFirstTimeTxt.text = "";
        }
        m_ValueTxt.SetText(entity.Value.TruncateValue());

        LootReward = new LootParams { Type = ELootType.Resource };
        if (save.IsBoughtFirstTime)
        {
            LootReward.Data = entity.GetValue();
        }
        else
        {
            LootReward.Data = entity.GetValueFirstTime();
        }
    }


    protected void ShowRewards()
    {
        var lootParams = new List<LootParams>() { LootReward };
        PanelManager.ShowRewards(lootParams);
    }
}