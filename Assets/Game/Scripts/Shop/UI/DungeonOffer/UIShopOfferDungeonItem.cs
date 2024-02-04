using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIShopOfferDungeonItem : UIShopOfferItemBase
{
    protected OfferItemSave save;
    protected OfferDungeonEntity entity;
    [SerializeField] protected TextMeshProUGUI m_TitleTxt;
    [SerializeField] protected TextMeshProUGUI m_DescriptionTxt;
    [SerializeField] protected TextMeshProUGUI m_XValueTxt;
    [SerializeField] protected Text m_PriceOriginTxt;
    [SerializeField] protected Text m_PriceSaleOffTxt;
    [SerializeField] protected UIInventorySlot[] m_SlotRewards;
    [SerializeField] protected Image m_FrameImage;
    [SerializeField] protected Button m_Button;

    private UIShopOfferDungeon uiParent;

    public void Init(UIShopOfferDungeon ui)
    {
        uiParent = ui;
    }

    public void SetData(OfferDungeonEntity entity, OfferItemSave save)
    {
        this.entity = entity;
        this.save = save;
        Setup();
    }

    private void OnEnable()
    {
        m_Button.onClick.AddListener(BoughtOnClicked);
    }
    private void OnDisable()
    {
        m_Button.onClick.RemoveListener(BoughtOnClicked);
    }

    private async void Setup()
    {
        foreach (var slot in m_SlotRewards)
        {
            slot.Clear();
            slot.GetComponent<Image>().sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Offer, $"Item_Frame_Dungeon_{entity.FrameColor}");
        }

        m_TitleTxt.text = I2Localize.GetLocalize($"Offer/Dungeon_Title_{entity.Id}");
        m_DescriptionTxt.text = I2Localize.GetLocalize($"Offer/Dungeon_Description_{entity.Id}");
        m_XValueTxt.text = $"{entity.XValue}x";
        if (entity.XValue == 1)
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_XValueTxt.transform.parent.gameObject.SetActive(true);
        }
        var price = IAPManager.Instance.GetPrice(entity.ProductId);
        var isoCode = IAPManager.Instance.GetIsoCurrencyCode(entity.ProductId);

        m_PriceOriginTxt.text = $"{(price * 100 / (100 - entity.SaleOff)).PriceShow()} {isoCode}";
        m_PriceSaleOffTxt.text = $"{price.PriceShow()} {isoCode}";

        m_FrameImage.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Offer, $"Frame_Dungeon_{entity.FrameColor}");

        var rewards = entity.Rewards;
        for (int i = 0; i < rewards.Count; i++)
        {
            if (i >= m_SlotRewards.Count()) break;
            var reward = rewards[i];
            var slot = m_SlotRewards[i];

            var pathLoot = string.Format(AddressableName.UILootItemPath, reward.Type);
            if (reward.Type == ELootType.Equipment)
            {
                pathLoot = AddressableName.UIGeneralEquipmentItem;
            }
            var lootItem = (await ResourcesLoader.Instance.GetAsync<UIGeneralBaseIcon>(pathLoot, slot.transform));
            lootItem.SetData(reward.Data);

            slot.SetItem(lootItem);
        }
    }

    public void BoughtOnClicked()
    {
        IAPManager.Instance.BuyProduct(entity.ProductId, OnCompletePurchased);
    }

    private void Purchased()
    {
        PanelManager.ShowRewards(entity.Rewards).Forget();
        save.Bought();
        uiParent.OnInit();
        Sound.Controller.Instance.PlayOneShot(AddressableName.SFX_Product_Bought);

        foreach (var rw in entity.Rewards)
        {
            // Track
            FirebaseAnalysticController.Tracker.NewEvent("buy_resource")
                .AddStringParam("item_category", rw.Type.ToString())
                .AddStringParam("item_id", (rw.Data as ResourceData).Resource.ToString())
                .AddStringParam("source", "dungeon_bundle")
                .AddStringParam("source_id", entity.ProductId)
                .AddDoubleParam("value", rw.Data.ValueLoot)
                .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource((rw.Data as ResourceData).Resource))
                .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn((rw.Data as ResourceData).Resource))
                .Track();
        }
    }

    private void OnCompletePurchased(IAPManager.PurchaseState status, IAPPackage product)
    {
        if (status == IAPManager.PurchaseState.Success)
        {
            if (product.id == entity.ProductId)
            {
                Purchased();
            }
        }
    }
}