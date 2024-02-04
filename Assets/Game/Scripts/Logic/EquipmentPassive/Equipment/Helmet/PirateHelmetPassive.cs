public class PirateHelmetPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epic_coinMul;
    public ValueConfigSearch legendary_coinMul;

    private StatModifier coinMul;

    public override void Play()
    {
        base.Play();
        coinMul = new StatModifier(EStatMod.Flat, 0);
        Caster.Stats.AddModifier(StatKey.CoinMul, coinMul, this);
        if (Rarity >= ERarity.Epic)
        {
            coinMul.Value = epic_coinMul.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            coinMul.Value = legendary_coinMul.FloatValue;
        }
    }
    public override void Remove()
    {
        base.Remove();
        Caster.Stats.RemoveModifiersFromSource(this);
    }
}