using Game.GameActor;

public class BazookaDronePassive : BaseDronePassive
{
    public ValueConfigSearch BaseDmg,IncreaseRate , CoolDownRate ;
    public override void Initialize(ActorBase actor)
    {
        base.Initialize(actor);
    }
    public override void Play()
    {
        base.Play();
    }

    protected override IStatGroup GetStatDrone(ERarity rarity)
    {
        var playerStats = GameSceneManager.Instance.PlayerData.Stats;
        var stats = DroneStat.Default();
        stats.SetBaseValue(StatKey.Dmg, playerStats.GetStat(StatKey.Dmg).Value);

        // drone attack dmg increase 100%
        stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, BaseDmg.FloatValue), this);

        for (int r=0;r<System.Enum.GetNames(typeof(ERarity)).Length;r++)
        {
            if (r <= (int)rarity)
            {
                switch (r)
                {
                    case (int)ERarity.Epic:
                        // drone attack dmg increase 100%
                        stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, IncreaseRate.FloatValue), this);
                        break;
                    case (int)ERarity.Legendary:
                        // drone skillcooldown decrease 33%
                        stats.AddModifier(StatKey.Cooldown, new StatModifier(EStatMod.PercentAdd, CoolDownRate.FloatValue), this);
                        break;
                }
            }
        }
       

        stats.CalculateStats();
        return stats;
    }
}
