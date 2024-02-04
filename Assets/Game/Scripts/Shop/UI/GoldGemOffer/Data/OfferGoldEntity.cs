using BansheeGz.BGDatabase;

[System.Serializable]
public class OfferGoldEntity : OfferBaseEntity
{
    public override EResource Resource => EResource.Gold;
    public ResourceData Cost;

    public OfferGoldEntity(BGEntity e) : base(e)
    {
        var cost = e.Get<int>("Cost");
        Cost = new ResourceData { Value = cost, Resource = EResource.Gem };
    }
}