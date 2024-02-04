using Cysharp.Threading.Tasks;
using System;
using System.Xml.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
public class UITooltipPanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private UIInventorySlot slotPanel;
    private ILootData data;
    public override void PostInit()
    {
    }

    public void Show(ILootData data)
    {
        this.data = data;
        Validate();
        base.Show();
    }

    private void Validate()
    {
        title.transform.parent.gameObject.SetActive(false);
        if (data is ResourceData)
        {
            ShowResource(data as ResourceData);
            return;
        }
        if (data is EquipmentData)
        {
            ShowEquipment(data as EquipmentData);
            return;
        }
        if (data is HeroData)
        {
            ShowHero(data as HeroData);
            return;
        }
        if (data is ExpData)
        {
            ShowExp(data as ExpData);
            return;
        }
        if (data is SkillTreeLootData)
        {
            ShowSkillTree(data as SkillTreeLootData);
        }
        if (data is BuffData)
        {
            ShowBuffTooltip(data as BuffData);
        }
    }

    private void ShowBuffTooltip(BuffData buffData)
    {
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Buff), buffData, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
            (t as UILootItemBase).SetText("");
        }).Forget();

        description.text = buffData.Description();
        title.transform.parent.gameObject.SetActive(true);
        title.text = buffData.GetName();
    }

    private void ShowSkillTree(SkillTreeLootData data)
    {
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Stat), data, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
            (t as UILootItemBase).SetText("");
        }).Forget();

        description.text = data.GetDescription();
        title.transform.parent.gameObject.SetActive(true);
        title.text = I2Localize.GetLocalize($"Stat/{data.statKey}");
    }

    private void ShowExp(ExpData expData)
    {
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Exp), expData, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
            (t as UILootItemBase).SetText("");
        }).Forget();

        description.text = I2Localize.GetLocalize("Use for/use_" + ELootType.Exp);
        title.transform.parent.gameObject.SetActive(true);
        title.text = I2Localize.GetLocalize($"Resource/Exp");
    }

    private void ShowHero(HeroData heroData)
    {
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Hero), heroData, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
        }).Forget();

        description.text = I2Localize.GetLocalize("Use for/use_" + heroData.HeroType + "_Hero");
    }

    private void ShowEquipment(EquipmentData equipmentData)
    {
        UIHelper.GetUILootIcon(AddressableName.UIGeneralEquipmentItem, equipmentData, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
        }).Forget();
        if (equipmentData.IsRandom)
        {
            var key = $"{equipmentData.Rarity}{equipmentData.Type}";
            description.text = I2Localize.GetLocalize("Use for/use_" + key);

            if (equipmentData.IsRandomType)
            {
                key = $"Equipment_{equipmentData.Rarity}";
                description.text = I2Localize.GetLocalize("Use for/use_" + key);
            }
        }
        else
        {

        }
    }

    private void ShowResource(ResourceData resourceData)
    {
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Resource), resourceData, slotPanel.transform).ContinueWith(t =>
        {
            slotPanel.SetItem(t);
            (t as UILootItemBase).SetText("");
        }).Forget();

        description.text = I2Localize.GetLocalize("Use for/use_" + resourceData.Resource.ToString());
        title.transform.parent.gameObject.SetActive(true);
        title.text = I2Localize.GetLocalize($"Resource/{resourceData.Resource}");
    }
}
