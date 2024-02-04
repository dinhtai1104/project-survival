namespace Game.GameActor.Buff
{
    public class TankerBuff : AbstractBuff
    {
        public override void Play()
        {
            Caster.Stats.AddModifier(StatKey.Hp,new StatModifier(EStatMod.PercentAdd, (float)GetValue(StatKey.Hp)),this);
            Caster.Heal(Caster.Stats.GetBaseValue(StatKey.Hp) * (float)GetValue(StatKey.Hp));
        }
       
    }
}