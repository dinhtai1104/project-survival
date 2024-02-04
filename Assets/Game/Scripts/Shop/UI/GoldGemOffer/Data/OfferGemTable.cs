[System.Serializable]
public class OfferGemTable : DataTable<int, OfferGemEntity>
{
    public override void GetDatabase()
    {
        DB_OfferGem.ForEachEntity(e =>
        {
            var offer = new OfferGemEntity(e);
            Dictionary.Add(offer.Id, offer);
        });
    }
}