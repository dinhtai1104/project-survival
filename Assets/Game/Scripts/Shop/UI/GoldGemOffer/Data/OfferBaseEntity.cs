using BansheeGz.BGDatabase;

[System.Serializable]
public class OfferBaseEntity
{
    public virtual EResource Resource { get; }
    public int Id;
    public int Value;
    public int ValueFirstTime;

    public OfferBaseEntity() { }
    public OfferBaseEntity(BGEntity e) : this()
    {
        Id = e.Get<int>("Id");
        Value = e.Get<int>("Value");
        ValueFirstTime = e.Get<int>("FirstTime");
    }

    public ResourceData GetValue()
    {
        return new ResourceData { Resource = Resource, Value = Value };
    }
    public ResourceData GetValueFirstTime()
    {
        return new ResourceData { Resource = Resource, Value = ValueFirstTime + Value };
    }
}