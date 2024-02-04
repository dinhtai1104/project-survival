using UnityEngine;

namespace Game.GameActor.Buff
{
    public class StressBuff : AbstractBuff
    {
        public ValueConfigSearch stress_HpTrigger = new ValueConfigSearch("Buff_Stress_HpTrigger");
        private bool isApplied = false;
        private StatModifier FireRateMod;
        public override void Play()
        {
            FireRateMod = BuffData.StatRefer.GetModifier(StatKey.FireRate);
        }
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isApplied)
            {
                if (Caster.HealthHandler.GetPercentHealth() > stress_HpTrigger.FloatValue)
                {
                    isApplied = false;
                    Debug.Log("Remove Stress");
                    Caster.Stats.RemoveModifiersFromSource(this);
                    Caster.Stats.CalculateStats();
                }
            }
            else
            {
                if (Caster.HealthHandler.GetPercentHealth() <= stress_HpTrigger.FloatValue)
                {
                    Debug.Log("Add Stress");
                    isApplied = true;
                    Caster.Stats.AddModifier(StatKey.Dmg, FireRateMod, this);
                    Caster.Stats.CalculateStats();
                }
            }

        }
    } 
}