using Game.GameActor;
using Game.GameActor.Buff;
using System.Collections.Generic;
using UnityEngine;

namespace Game.BuffCard
{
    public class PiercingAmmoBuff : AbstractBuff
    {
        public float[] dmgReduceRates = { 1,0.6f,0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f};
        private void OnEnable()
        {
            Messenger.AddListener<BulletBase,  ModifierSource>(EventKey.BulletImpact, OnBulletImpact);
            Messenger.AddListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);

        }
        private void OnDisable()
        {
            Messenger.RemoveListener<BulletBase, ModifierSource>(EventKey.BulletImpact, OnBulletImpact);
            Messenger.RemoveListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
        }

        private void OnBulletImpact(BulletBase bulletBase, ModifierSource modifier)
        {
          
            modifier.Stat.AddModifier(new StatModifier(EStatMod.Percent, dmgReduceRates[Mathf.Min(bulletBase.currentPiercing, dmgReduceRates.Length - 1)]));
            modifier.Stat.RecalculateValue();

        }
        void OnPreFire(ActorBase actor, BulletBase bullet, List<ModifierSource> modifiers)
        {

            if (actor == Caster)
            {
                if (modifiers.Count >= 2)
                {
                    modifiers[1].Value = 1 + (BuffData.Level + 1) * (int)GetValue(StatKey.Number);
                    modifiers[1].Stat.RecalculateValue();
                }
            }
        }


        public override void Play()
        {
        
        }
    }
}