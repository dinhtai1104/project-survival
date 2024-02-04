using Assets.Game.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Equipment Icon 
/// </summary>
public class UIGeneralEquipmentIcon : UIGeneralBaseIcon
{
    [SerializeField] protected Image equipmentRarityImg;
    [SerializeField] protected Image equipmentIconImg;
    [SerializeField] protected Image equipmentTypeImg;
    [SerializeField] protected TextMeshProUGUI equipmentLevelImg;
    [SerializeField] protected GameObject lockedFrameObj;
    [SerializeField] protected GameObject pickedFrameObj;

    public override Sprite Sprite => equipmentIconImg.sprite;

    protected bool isPicked = false;
    protected bool isLocked = false;
    public bool IsLocked => isLocked;
    public bool IsPicked => isPicked;

    public EquipmentData equimentData;

    [SerializeField]
    protected EquipableItem item;
    public EquipableItem Item => item;

    public void SetLocked(bool IsLocked)
    {
        this.isLocked = IsLocked;
        if (lockedFrameObj != null)
        {
            lockedFrameObj.SetActive(isLocked);
        }
    }

    public void SetPicked(bool IsPicked)
    {
        this.isPicked = IsPicked;
        if (pickedFrameObj != null)
        {
            pickedFrameObj.SetActive(isPicked);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        SetInformation();
        SetLocked(false);
        SetPicked(false);
        ParentSlot.SetRarity(this.equimentData.Rarity);
    }

    public virtual void Init(EquipableItem item)
    {
        this.item = item;
        equimentData = new EquipmentData
        {
            IdString = item.Id,
            EquipmentEntity = DataManager.Base.Equipment.Get(item.Id).Clone(),
            Level = item.ItemLevel,
            Rarity = item.EquipmentRarity,
            Type = item.EquipmentType
        };

        SetInformation();
        SetLocked(false);
        SetPicked(false);
    }
    public virtual void SetInformation()
    {
        if (equimentData == null)
        {
            equimentData = new EquipmentData
            {
                IdString = item.Id,
                EquipmentEntity = DataManager.Base.Equipment.Get(item.Id).Clone(),
                Level = item.ItemLevel,
                Rarity = item.EquipmentRarity,
                Type = item.EquipmentType
            };
        }
        else
        {
            equimentData.IdString = item.Id;
            equimentData.EquipmentEntity = DataManager.Base.Equipment.Get(item.Id).Clone();
            equimentData.Level = item.ItemLevel;
            equimentData.Rarity = item.EquipmentRarity;
            equimentData.Type = item.EquipmentType;
        }

        if (equimentData != null)
        {
            if (equipmentIconImg == null) return;
            var icon = ResourcesLoader.Instance.GetSprite(AtlasName.Equipment, $"{equimentData.IdString}");
            equipmentIconImg.sprite = icon;
            if (!equimentData.IsRandomType)
            {
                equipmentTypeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{equimentData.Type}");
            }
            else
            {
                equipmentTypeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{-1}");
            }

            equipmentRarityImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{equimentData.Rarity}");
            equipmentLevelImg.text = $"Lvl {equimentData.Level}";
            if (equimentData.Level == 0)
            {
                equipmentLevelImg.text = "";
            }
        }
    }

    public virtual void SetIcon(Sprite icon)
    {
        equipmentIconImg.sprite = icon;
    }
    public virtual void SetLevel(int level)
    {
        equipmentLevelImg.text = "Lvl " + level.ToString();
    }
    public virtual void SetEquipmentRarity(Sprite rarity)
    {
        equipmentRarityImg.sprite = rarity;
    }
    public virtual void SetEquipmentType(Sprite type)
    {
        equipmentTypeImg.sprite = type;
    }

    public override void SetData(ILootData lootData)
    {
        var equi = lootData as EquipmentData;
        equimentData = equi;
        if (equi.Id == -1)
        {
            item = null;
            var icon = ResourcesLoader.Instance.GetSprite(AtlasName.Equipment, $"{equi.IdString}");
            equipmentIconImg.sprite = icon;

            if (!equimentData.IsRandomType)
            {
                equipmentTypeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{equi.Type}");
            }
            else
            {
                equipmentTypeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{-1}");
            }

            equipmentRarityImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{equi.Rarity}");
            equipmentLevelImg.text = "";

            return;
        }
        Init(equi.GetItem());
    }
}