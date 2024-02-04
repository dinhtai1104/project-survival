using TMPro;
using UnityEngine;

public class UIShopOfferGem : UIShopOffer
{
    private OfferGemTable gemTable;
    private OfferSave gemSave;

    public override async void OnInit()
    {
        base.OnInit();
        gemTable = DataManager.Base.OfferGem;
        gemSave = DataManager.Save.Offer.GemSave;

        foreach (var offer in gemTable.Dictionary)
        {
            var itemIns = await ResourcesLoader.Instance.GetAsync<UIShopOfferGemItem>("Common/UIShopGemItem.prefab", contentTrans);
            itemIns.SetData(offer.Value, gemSave.GetItem(offer.Key));
        }
    }
}