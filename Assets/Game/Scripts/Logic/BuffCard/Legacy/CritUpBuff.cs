namespace Game.GameActor.Buff
{
    public class CritUpBuff : AbstractBuff
    {
        public override void Play()
        {
            var stat = BuffData.StatRefer.GetModifier(StatKey.CritRate);
            Caster.Stats.AddModifier(StatKey.CritRate, stat, this);
        }
    }
}