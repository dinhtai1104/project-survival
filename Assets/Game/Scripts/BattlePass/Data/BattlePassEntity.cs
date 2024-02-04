using BansheeGz.BGDatabase;

[System.Serializable]
public class BattlePassEntity
{
    public int Id;
    public int Level;
    public int ExpRequire;

    public LootParams FreeReward;
    public LootParams PremiumReward;

    public BattlePassEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Level = e.Get<int>("Level");
        ExpRequire = e.Get<int>("ExpRequire");

        var freeData = e.Get<string>("Free");
        var premiumData = e.Get<string>("Premium");

        FreeReward = new LootParams(freeData);
        PremiumReward = new LootParams(premiumData);
    }
}