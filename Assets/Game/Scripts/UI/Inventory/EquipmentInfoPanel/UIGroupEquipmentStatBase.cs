using TMPro;
using UnityEngine;

public class UIGroupEquipmentStatBase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private TextMeshProUGUI rarityTxt;
    [SerializeField] private UIAffixText mainStatTxt;

    public void Show(EquipableItem item)
    {
        typeTxt.text = I2Localize.GetLocalize($"Common/{item.EquipmentType}");
        levelTxt.text = I2Localize.GetLocalize($"Common/LevelEquipment_Title", item.ItemLevel);
        rarityTxt.text = I2Localize.GetLocalize($"Common/{item.EquipmentRarity}");

        mainStatTxt.SetDescription(item.BaseStatAffix.GetDescription());
    }
}