using Assets.Game.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryGeneralIconType : MonoBehaviour, IInventorySlotType
{
    public Image equimentType;
    public GameObject equipmentIcon;
    public GameObject rarityBg;
    public void SetType(UIGeneralBaseIcon icon)
    {
        equipmentIcon.SetActive(false);
        //rarityBg.SetActive(false);

        equimentType.enabled = false;
        var equipment = icon as UIGeneralEquipmentIcon;
        if (equipment != null)
        {
            //rarityBg.SetActive(true);
            equipmentIcon.SetActive(true);
            equimentType.enabled = true;
            if (!equipment.equimentData.IsRandomType)
            {
                equimentType.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{equipment.equimentData.Type}");
            }
            else
            {
                equimentType.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{-1}");
            }
        }
    }
}
