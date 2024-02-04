using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class ShopResourceEntity
{
    public EResource Resource;
    public int Value;
    public ResourceData Buy;
    public ResourceData Cost;

    private BGEntity entity;

    public ShopResourceEntity(BGEntity e)
    {
        entity = e;
        Enum.TryParse(e.Get<string>("Resource"), out Resource);
        Value = e.Get<int>("Value");

        Buy = new ResourceData(Resource, Value);
        Cost = new LootParams(e.Get<string>("Cost")).Data as ResourceData;
    }
    public ShopResourceEntity Clone()
    {
        return new ShopResourceEntity(entity);
    }
}