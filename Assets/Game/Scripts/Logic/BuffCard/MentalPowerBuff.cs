using UnityEngine;

namespace Game.GameActor.Buff
{
    public class MentalPowerBuff : AbstractBuff
    {
        public ValueConfigSearch mentalPower_HpTrigger = new ValueConfigSearch("Buff_MentalPower_HpTrigger");
        private bool isApplied = false;
        private StatModifier DmgMod;
        public override void Play()
        {
            DmgMod = BuffData.StatRefer.GetModifier(StatKey.Dmg);
        }
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isApplied)
            {
                if (Caster.HealthHandler.GetPercentHealth() < mentalPower_HpTrigger.FloatValue)
                {
                    isApplied = false;
                    Debug.Log("Remove Stress");
                    Caster.Stats.RemoveModifiersFromSource(this);
                    Caster.Stats.CalculateStats();
                }
            }
            else
            {
                if (Caster.HealthHandler.GetPercentHealth() >= mentalPower_HpTrigger.FloatValue)
                {
                    Debug.Log("Add Stress");
                    isApplied = true;
                    Caster.Stats.AddModifier(StatKey.Dmg, DmgMod, this);
                    Caster.Stats.CalculateStats();
                }
            }

        }
    }
}