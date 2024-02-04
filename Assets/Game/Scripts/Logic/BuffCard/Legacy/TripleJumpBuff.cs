namespace Game.GameActor.Buff
{
    public class TripleJumpBuff : AbstractBuff
    {
        public override void Play()
        {
            var stat = BuffData.StatRefer.GetModifier(StatKey.JumpCount);
            Caster.Stats.AddModifier(StatKey.JumpCount, stat, this);
        }
    }
}