using Game.GameActor.Buff;

namespace Game.GameActor.Passive
{
    public class JumpCatPassive : AbstractBuff
    {
        public override void Play()
        {
            Caster.Stats.AddModifier(StatKey.JumpCount, new StatModifier(EStatMod.Flat, 1), this);
        }
    }
}