using UnityEngine;

namespace Game.GameActor.Buff
{
    public class SpeedUpBuff : AbstractBuff
    {
        public override void Play()
        {
            var stat = BuffData.StatRefer.GetModifier(StatKey.SpeedMove);
            try
            {
                Caster.Stats.AddModifier(StatKey.SpeedMove, stat, this);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
            Caster.Stats.CalculateStats();
            Debug.Log($"SpeedMove after caster {BuffEntity.Type}: " + Caster.Stats.GetValue(StatKey.SpeedMove));
        }
    }
}