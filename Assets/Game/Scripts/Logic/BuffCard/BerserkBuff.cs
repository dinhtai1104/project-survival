using UnityEngine;

namespace Game.GameActor.Buff
{
    public class BerserkBuff : AbstractBuff
    {
        public ValueConfigSearch berserk_HpTrigger = new ValueConfigSearch("Buff_Berserk_HpTrigger");
        private bool isApplied = false;
        private StatModifier DmgAdd;
        public override void Play()
        {
            DmgAdd = BuffData.StatRefer.GetModifier(StatKey.Dmg);
        }
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isApplied)
            {
                if (Caster.HealthHandler.GetPercentHealth() > berserk_HpTrigger.FloatValue)
                {
                    isApplied = false;
                    Debug.Log("Remove Berserk");
                    Caster.Stats.RemoveModifiersFromSource(this);
                    Caster.Stats.CalculateStats();
                }
            }
            else
            {
                if (Caster.HealthHandler.GetPercentHealth() <= berserk_HpTrigger.FloatValue)
                {
                    Debug.Log("Add Berserk");
                    isApplied = true;
                    Caster.Stats.AddModifier(StatKey.Dmg, DmgAdd, this);
                    Caster.Stats.CalculateStats();
                }
            }
            
        }
    }
}