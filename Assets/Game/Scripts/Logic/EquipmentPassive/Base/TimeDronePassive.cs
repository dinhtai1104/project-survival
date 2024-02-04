using Game.GameActor;

public class TimeDronePassive : BaseDronePassive
{
    public ValueConfigSearch IncreaseRate ,CoolDownRate;
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

        stats.AddStat(StatKey.TimeSlowAddictional, 0);

        for (int r = 0; r < System.Enum.GetNames(typeof(ERarity)).Length; r++)
        {
            if (r <= (int)rarity)
            {
                switch (r)
                {
                    case (int)ERarity.Epic:
                        // drone slow time increase 100% = 10s
                        stats.AddModifier(StatKey.TimeSlowAddictional, new StatModifier(EStatMod.Flat, IncreaseRate.FloatValue), this);
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
