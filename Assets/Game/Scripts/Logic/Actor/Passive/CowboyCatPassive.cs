using Game.GameActor.Buff;

namespace Game.GameActor.Passive
{
    public class CowboyCatPassive : AbstractBuff
    {
        public override void Play()
        {
            if (Caster != null)
            {
                Caster.Stats.AddModifier(StatKey.FireRate, new StatModifier(EStatMod.PercentMul, GetValue(StatKey.FireRate)), this);
            }
        }
    }
}