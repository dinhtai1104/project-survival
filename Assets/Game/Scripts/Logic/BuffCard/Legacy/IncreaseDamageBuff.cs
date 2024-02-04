using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class IncreaseDamageBuff : AbstractBuff
    {
        public override void Play()
        {
            var stat = BuffData.StatRefer.GetModifier(StatKey.Dmg);
            try
            {
                Caster.Stats.AddModifier(StatKey.Dmg, stat, this);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
            Caster.Stats.CalculateStats();
            Debug.Log($"Dmg after caster {BuffEntity.Type}: " + Caster.Stats.GetValue(StatKey.Dmg));
        }
    }
}