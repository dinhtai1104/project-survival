using BansheeGz.BGDatabase;

[System.Serializable]
public class PlayGiftEntity
{
    public int Id;
    public LootParams Reward;
    public string ProductId;

    public PlayGiftEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Reward = new LootParams(e.Get<string>("Reward"));

        switch (Reward.Type)
        {
            case ELootType.Resource:
                Reward.Type = ELootType.Resource;
                var res = Reward.Data as ResourceData;
                if (res.Resource >= EResource.MainWpFragment && res.Resource <= EResource.BootFragment)
                {
                    Reward.Type = ELootType.Fragment;
                }
                if (res.Resource >= EResource.NormalHero && res.Resource <= EResource.EvilHero)
                {
                    Reward.Type = ELootType.HeroFragment;
                }
                if (res.Resource >= EResource.BaseStone && res.Resource <= EResource.LightStone)
                {
                    Reward.Type = ELootType.HeroStone;
                }
                break;
            case ELootType.Equipment:
                Reward.Type = ELootType.Equipment;
                break;
        }

        ProductId = e.Get<string>("ProductId");
    }
}