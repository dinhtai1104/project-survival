using Game.GameActor;

public class BoneHelmetPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epic_HealMulAdd;
    private StatModifier healAdd;
    public override void Play()
    {
        base.Play();
        healAdd = new StatModifier(EStatMod.Flat, epic_HealMulAdd.FloatValue);
        Caster.Stats.AddModifier(StatKey.HealMul, healAdd, this);
    }
    public override void Remove()
    {
        base.Remove();
        Caster.Stats.RemoveModifiersFromSource(this);
    }
}