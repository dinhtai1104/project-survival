[System.Serializable]
public class OfferGoldTable : DataTable<int, OfferGoldEntity>
{
    public override void GetDatabase()
    {
        DB_OfferGold.ForEachEntity(e =>
        {
            var offer = new OfferGoldEntity(e);
            Dictionary.Add(offer.Id, offer);
        });
    }
}