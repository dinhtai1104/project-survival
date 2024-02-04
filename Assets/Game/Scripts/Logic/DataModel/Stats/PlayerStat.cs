using UnityEngine;

[System.Serializable]
public class PlayerStat : StatGroup
{
    public static PlayerStat Default()
    {
        var stats = new PlayerStat();

        stats.AddStat(StatKey.Hp, 0);
        stats.AddStat(StatKey.Dmg, 0);
        stats.AddStat(StatKey.CritDmg, 0, 0f);
        stats.AddStat(StatKey.CritRate, 0, 0, 1f);
        stats.AddStat(StatKey.SpeedMove, 0, 0f);
        stats.AddStat(StatKey.JumpForce, 0, 0f);
        stats.AddStat(StatKey.JumpCount, 0, 0f, 5);

        stats.AddStat(StatKey.FireRate, 0, 0.1f);
        // headshot rate -> one kill per shot
        stats.AddStat(StatKey.HeadshotRate, 0, 0, 1f);

        stats.AddStat(StatKey.DodgeRate, 0, 0, 1f);
        // Number bullet per shoot
        stats.AddStat(StatKey.BulletPerShot, 1, 1);
        // Number revive available
        stats.AddStat(StatKey.Revive, 0, 0);
        // Size player
        stats.AddStat(StatKey.Size, 1, 0.25f);
        // mul heal
        stats.AddStat(StatKey.HealMul, 1, 0);

        stats.AddStat(StatKey.BulletBackNumber, 0, 0);
        stats.AddStat(StatKey.BulletReflectNumber, 0, 0);


        stats.AddStat(StatKey.AttackSpeed, 0);
        stats.AddStat(StatKey.RatePoison, 0);
        stats.AddStat(StatKey.ClimbDrag, 0, 0, 50);

        stats.AddStat(StatKey.ReduceDmg, 0, 0, 0.9f);

        stats.AddStat(StatKey.CoinMul, 1, 0);
        return stats;
    }
}
