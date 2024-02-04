using Assets.Game.Scripts.Utilities;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Slot grid show item 
/// </summary>
public class UIInventorySlot : MonoBehaviour, IRaritySlot
{
    public delegate void InventorySlotOnClickItem(UIInventorySlot slot, UIGeneralBaseIcon item);
    [SerializeField] private RectTransform holder;
    [SerializeField] private Button button;
    [SerializeField] private Image raritySlot;
    [SerializeField] private Image rarityTypeSlot;
    [SerializeField] private Transform shinyParent;
    [SerializeField, ReadOnly] private UIGeneralBaseIcon item;
    [SerializeField] private GameObject notiObject;
    [SerializeField] private TextMeshProUGUI stackAmountTxt;

    private GameObject frameSlot;
    private UIShinyInventorySlot shinySlot;
    public RectTransform Holder => holder;


    public InventorySlotOnClickItem OnItemClick;
    private IInventorySlotType inventoryType;
    private bool isShowRarity = true;

    private ERarity rarity;
    public ERarity Rarity => rarity;

    private void Awake()
    {
        //isShowRarity = raritySlot.gameObject.activeSelf;
    }

    private void OnEnable()
    {
        SetNoti(false);
    }

    public void SetNoti(bool value)
    {
        if (notiObject != null)
        {
            notiObject.SetActive(value);
        }
    }
    public void SetStack(int value)
    {
        if (stackAmountTxt == null) return;
        if (value <= 1) stackAmountTxt.text = "";
        else
        {
            stackAmountTxt.text = value.TruncateValue();
        }
    }

    public async void SetRarity(ERarity rarity)
    {
        this.rarity = rarity;
        try
        {
            if (raritySlot)
            {
                this.raritySlot.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{rarity}");
            }
            if (rarityTypeSlot)
            {
                this.rarityTypeSlot.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, $"{rarity}_Type");
            }
        } 
        catch (Exception e)
        {

        }

        var path = "Frame/Frame_{0}.prefab".AddParams(rarity);
        switch (rarity)
        {
            case ERarity.Epic:
            case ERarity.Legendary:
            case ERarity.Ultimate:
                frameSlot = await ResourcesLoader.Instance.GetGOAsync(path, raritySlot.transform);
                shinySlot = await ResourcesLoader.Instance.GetAsync<UIShinyInventorySlot>("Frame/Frame_Shiny.prefab", shinyParent);
                shinySlot.SetColor(RarityExtension.GetColor(rarity));
                break;
        }
    }

    public void SetItem(UIGeneralBaseIcon item)
    {
        SetStack(0);
        Clear();
        SetRarity(ERarity.Common);

        if (item as UIGeneralEquipmentIcon)
        {
            raritySlot.gameObject.SetActive(true);
        }
        else
        {
         //   raritySlot.gameObject.SetActive(isShowRarity);
        }

        this.item = item;
        item.transform.SetParent(holder.transform, false);
        item.SetSlot(this);
        button.onClick.AddListener(InventorySlotOnClick);

        // Set Type Inventory Slot
        inventoryType = GetComponent<IInventorySlotType>();
        if (inventoryType != null)
        {
            inventoryType.SetType(item);
        }
        item.OnUpdate();
        item.transform.localPosition = Vector3.zero;
    }
    public void Clear()
    {
        SetStack(0);
        SetRarity(ERarity.Common);
        if (item != null)
        {
            item.Clear();
            PoolManager.Instance.Despawn(item.gameObject);
            item = null;

            button.onClick.RemoveListener(InventorySlotOnClick);

            if (frameSlot != null)
            {
                PoolManager.Instance.Despawn(frameSlot.gameObject);
                frameSlot = null;
            }

            if (shinySlot != null)
            {
                PoolManager.Instance.Despawn(shinySlot.gameObject);
                shinySlot = null;
            }
        }
        else
        {
            
        }
    }

    private void InventorySlotOnClick()
    {
        OnItemClick?.Invoke(this, item);
    }

    public void ActiveRarity(bool active)
    {
        raritySlot.enabled = active;
    }

    public T Get<T>() where T : UIGeneralBaseIcon
    {
        return Item as T;
    }

    public UIGeneralBaseIcon Item => item;

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}