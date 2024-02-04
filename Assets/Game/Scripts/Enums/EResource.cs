public enum EResource : int
{
    None = -1,
    // Currency
    Gold = 0,
    Gem,
    Energy,

    //Key
    SilverKey = 100,
    GoldenKey,
    HeroKey,

    // AttributeStone
    BaseStone = 200,
    PoisonStone,
    FrozenStone,
    DarkStone,
    WeaponStone,
    LightStone,

    // AttributeHero
    NormalHero = 300,
    PoisonHero,
    FrozenHero,
    NinjaHero,
    JumpHero,
    RocketHero,
    ShinigamiHero,
    CowboyHero,
    AngelHero,
    EvilHero,

    // AttributeEquipment
    MainWpFragment = 400,
    DroneFragment,
    HelmetFragment,
    ArmorFragment,
    NecklaceFragment,
    BootFragment,

    // Offer
    ReviveCard = 1000,


    // Random
    EquipmentRdFragment = 10024,
    HeroStoneRdFragment = 10025,
    HeroRdFragment = 10026,

    // Daily Quest
    DailyQuestMedal = 20000,

    // Experience
    Exp = 400000,

    // PigCoin
    PigCoin = 50000,
}


public static class ResourceExtension
{
    public static string GetLocalize(this EResource resouce)
    {
        return I2Localize.GetLocalize($"Resource/{resouce}");
    }
}