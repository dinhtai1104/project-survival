using Assets.Game.Scripts.Utilities;
using System.IO;
using UnityEngine;

public class UIGroupEquipmentStatAffixes : MonoBehaviour
{
    private UIAffixText affixTextPrefab;
    [SerializeField] private UIGroupAffixText rareAffixParent;
    [SerializeField] private UIGroupAffixText epicAffixParent;
    [SerializeField] private UIGroupAffixText legendaryAffixParent;
    [SerializeField] private UIGroupAffixText ultimateAffixParent;
    [SerializeField] private Color affixUnlockColor = Color.white;
    [SerializeField] private Color affixLockColor = Color.white;
    public async void Show(EquipableItem item)
    {
        affixTextPrefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIAffixText)).GetComponent<UIAffixText>();
        SetAffix(ERarity.Rare, item, rareAffixParent);
        SetAffix(ERarity.Epic, item, epicAffixParent);
        SetAffix(ERarity.Legendary, item, legendaryAffixParent);
        SetAffix(ERarity.Ultimate, item, ultimateAffixParent);
    }
    public void Clear()
    {
        rareAffixParent.Clear();
        epicAffixParent.Clear();
        legendaryAffixParent.Clear();
        ultimateAffixParent.Clear();
    }
    private void SetAffix(ERarity rarity, EquipableItem item, UIGroupAffixText holder)
    {
        holder.Clear();
        string key;
        if (item.LineAffixes[rarity] == NullAffix.Null)
        {
            return;
        }
        var affixIns = PoolManager.Instance.Spawn(affixTextPrefab, holder.transform);

        if (item.EquipmentRarity < rarity)
        {
          //  key = I2Localize.GetLocalize("Common/UnlockAtRarity_Title", I2Localize.GetLocalize("Common/" + rarity));
          //  affixIns.SetDescription(key);
            affixIns.SetIcon(ResourcesLoader.Instance.GetSprite(AtlasName.Affix, $"{rarity}_Lock"));
            affixIns.SetColor(affixLockColor);
            affixIns.UseOutline(false);
        //    holder.AddAffixText(affixIns);
         //   return;
        }
        else
        {
            affixIns.UseOutline(true);
            affixIns.SetIcon(ResourcesLoader.Instance.GetSprite(AtlasName.Affix, $"{rarity}"));
            affixIns.SetColor(affixUnlockColor);
        }
        var affix = item.LineAffixes[rarity];
        key = affix.GetDescription();
        affixIns.SetDescription(key);

        holder.AddAffixText(affixIns);
    }
}