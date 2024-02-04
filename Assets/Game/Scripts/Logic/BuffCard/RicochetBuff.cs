using Game.GameActor;
using Game.GameActor.Buff;
using System.Collections.Generic;
using UnityEngine;

namespace Game.BuffCard
{
    public class RicochetBuff : AbstractBuff
    {
        public float[] dmgReduceRates = { 1, 0.65f, 0.4f, 0.2f, 0.2f, 0.2f };
        private void OnEnable()
        {
            Messenger.AddListener<BulletBase, ModifierSource>(EventKey.BulletImpact, OnBulletImpact);
            Messenger.AddListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);

        }
        private void OnDisable()
        {
            Messenger.RemoveListener<BulletBase,  ModifierSource>(EventKey.BulletImpact, OnBulletImpact);
            Messenger.RemoveListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
        }

        private void OnBulletImpact(BulletBase bulletBase,  ModifierSource modifier)
        {
            modifier.Stat.AddModifier(new StatModifier(EStatMod.Percent, /*bulletBase.currentRicochet==-1?1:*/dmgReduceRates[Mathf.Min(bulletBase.currentRicochet, dmgReduceRates.Length - 1)]));
            modifier.Stat.RecalculateValue();

        }
        void OnPreFire(ActorBase actor, BulletBase bullet, List<ModifierSource> modifiers)
        {
            if (actor == Caster)
            {
                modifiers[4].Stat.AddModifier(new StatModifier(EStatMod.Flat, (BuffData.Level + 1) * (int)GetValue(StatKey.Number)));
                modifiers[4].Stat.RecalculateValue();

                bullet.GetComponent<Sensor>().detectRange=(float)GetValue(StatKey.Range);
            }
        }


        public override void Play()
        {

        }
    }
}