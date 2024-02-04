public class UIShopOfferGold : UIShopOffer
{
    private OfferGoldTable goldTable;
    private OfferSave goldSave;

    public override async void OnInit()
    {
        base.OnInit();
        goldTable = DataManager.Base.OfferGold;
        goldSave = DataManager.Save.Offer.GoldSave;

        foreach (var offer in goldTable.Dictionary)
        {
            var itemIns = await ResourcesLoader.Instance.GetAsync<UIShopOfferGoldItem>("Common/UIShopGoldItem.prefab", contentTrans);
            itemIns.SetData(offer.Value, goldSave.GetItem(offer.Key));
        }
    }
}