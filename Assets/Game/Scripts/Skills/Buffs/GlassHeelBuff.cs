using Assets.Game.Scripts.Core.BuffHandler;
using Assets.Game.Scripts.Events;
using Core;
using Engine;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Buffs
{
    public class GlassHeelBuff : BuffGameObject
    {
        private StatModifier m_SpeedDecreaseModifier;
        private float m_MaxValue;

        protected override void OnInit()
        {
            base.OnInit();
            GameArchitecture.GetService<IEventMgrService>().Subscribe<DamageAfterHitEventArgs>(DamageAfterHitEventHandler);
            GameArchitecture.GetService<IEventMgrService>().Subscribe<HealthZeroEventArgs>(HealthZeroEventHandler);
        }

        protected override void OnExit()
        {
            base.OnExit();
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<DamageAfterHitEventArgs>(DamageAfterHitEventHandler);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<HealthZeroEventArgs>(HealthZeroEventHandler);
        }

        private void HealthZeroEventHandler(object sender, IEventArgs e)
        {
            var evt = e as HealthZeroEventArgs;
            var target = evt.m_Actor;
            if (target != null)
            {
                target.Stats.RemoveModifiersFromSource(this);
            }
        }

        private void DamageAfterHitEventHandler(object sender, IEventArgs e)
        {
            var evt = e as DamageAfterHitEventArgs;
            var attacker = evt.attacker;
            var defender = evt.defender;

            if (attacker != Owner) return;
            if (defender != null)
            {
                var mod = defender.Stats.GetModifiersFromSource(StatKey.Speed, this);
                var sum = mod.Sum(t => t.Value);
                if (sum <= m_MaxValue - m_SpeedDecreaseModifier.Value)
                {
                    defender.Stats.AddModifier(StatKey.Speed, m_SpeedDecreaseModifier, this);
#if DEVELOPMENT
                    Debug.Log($"Enemy Decrease SpeedMove | From {Owner.name} to {defender.name} ==> SpeedMove Now: {defender.Stats.GetValue(StatKey.Speed)} | using by " + name, defender.gameObject);
#endif
                }
            }
        }
    }
}
