using BansheeGz.BGDatabase;
using System.Collections.Generic;

[System.Serializable]
public class OfferDungeonEntity
{
    public int Id;
    public int DungeonId;
    public int SaleOff;
    public int XValue;
    public string ProductId;
    public IAPPackage PackageProduct;
    public string FrameColor;

    public List<LootParams> Rewards;

    public OfferDungeonEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        DungeonId = e.Get<int>("DungeonId");
        SaleOff = e.Get<int>("SaleOff");
        ProductId = e.Get<string>("ProductId");
        XValue = e.Get<int>("XValue");
        FrameColor = e.Get<string>("FrameColor");
        Rewards = new List<LootParams>();
        var lootRewardDatas = e.Get<List<string>>("Rewards");
        foreach (var data in lootRewardDatas)
        {
            var loot = new LootParams(data);
            Rewards.Add(loot);
        }
        PackageProduct = DataManager.Base.IapConfig.FindPackage(ProductId);
    }
}