using BansheeGz.BGDatabase;

[System.Serializable] 
public class OfferGemEntity : OfferBaseEntity
{
    public override EResource Resource => EResource.Gem;
    public string ProductId;

    public OfferGemEntity(BGEntity e) : base(e)
    {
        ProductId = e.Get<string>("ProductId");
    }
}