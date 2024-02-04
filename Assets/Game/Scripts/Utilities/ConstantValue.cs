using System.Collections.Generic;

public static class ConstantValue
{
    public static readonly int MinHeroPassiveBuff = 1000;
    public static readonly int MinEquipmentPassiveBuff = 2000;
    public static readonly int MaxStarHeroUpgrade = 5;

    public static readonly int DungeonMax = 5;
    public static readonly int DungeonEvent_MaxAdDay = 2;

    public static Dictionary<EHero, List<EBuff>> HeroExceptionBuff = new Dictionary<EHero, List<EBuff>>
    {
        {EHero.NormalHero, new List<EBuff>() },
        {EHero.CowboyHero, new List<EBuff>() },
        {EHero.NinjaHero, new List<EBuff>() { EBuff.WallClimb } },
        {EHero.ShinigamiHero, new List<EBuff>() },
        {EHero.AngelHero, new List<EBuff>() { EBuff.AngelWhisper } },
        {EHero.PoisonHero, new List<EBuff>() { } },
        {EHero.FrozenHero, new List<EBuff>() },
        {EHero.RocketHero, new List<EBuff>() { EBuff.AngelWings } },
        {EHero.JumpHero, new List<EBuff>() { EBuff.TripleJump } },
        {EHero.EvilHero, new List<EBuff>() { EBuff.LifeSteal } },
    };
}