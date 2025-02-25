using Assets.Game.Scripts.Events;
using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Manager
{
    public class KnockbackListener : MonoBehaviour
    {
        private void OnEnable()
        {
            GameArchitecture.GetService<IEventMgrService>().Subscribe<DamageAfterHitEventArgs>(KnockListener);
        }
        private void OnDisable()
        {
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<DamageAfterHitEventArgs>(KnockListener);
        }

        private void KnockListener(object sender, IEventArgs e)
        {
            var evt = e as DamageAfterHitEventArgs;
            if (evt.defender is not EnemyActor) return;
            var enemy = evt.defender as EnemyActor;
            var massDefender = enemy.EntityData.Mass;
            var knockback = evt.attacker.Stats.GetValue(StatKey.Knockback);
            var directionAttack = (enemy.CenterPosition - evt.attacker.CenterPosition).normalized;

            var dest = enemy.BotPosition + directionAttack * knockback / massDefender;
            enemy.Movement.MoveTween(dest, 0.2f);
        }
    }
}
