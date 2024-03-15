using Assets.Game.Scripts.Events;
using Core;
using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using Engine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class DamageNumberHandler : MonoBehaviour
    {
        //cooldown between get hit, time until reset damage to 0
        public float damageTextCoolDown = 0.1f;
        public Dictionary<ActorBase, DamageInfo> dict = new Dictionary<ActorBase, DamageInfo>();
        [ShowInInspector]
        public DamageNumber[] prefabs;

        private void OnEnable()
        {
            Architecture.Get<EventMgr>().Subscribe<DamageAfterHitEventArgs>(AttackEventHandler);
        }

        private void AttackEventHandler(object sender, IEventArgs e)
        {
            var evt = e as DamageAfterHitEventArgs;

            if (!dict.ContainsKey(evt.defender))
            {
                dict.Add(evt.defender, new DamageInfo(evt.defender));
            }
            DamageInfo damageInfo = dict[evt.defender];

            //if (Time.time - damageInfo.lastHitTime >= damageTextCoolDown)
            //{
            //    damageInfo.Clear();
            //}
            damageInfo.damage = evt.hitResult.Damage;
            //damageInfo.lastHitTime = Time.time;

            int direction = evt.attacker.CenterPosition.x < evt.defender.CenterPosition.x ? 1 : -1;
            if (evt.hitResult.Critical)
            {
                damageInfo.ShowDamage(direction, prefabs[1]);
            }
            else
            {
                damageInfo.ShowDamage(direction, prefabs[0]);
            }

        }

        private void OnDisable()
        {
            Architecture.Get<EventMgr>().Unsubscribe<DamageAfterHitEventArgs>(AttackEventHandler);
        }
    }
}

namespace Manager
{
    [Serializable]
    public class DamageInfo
    {
        public float damage;
        public float lastHitTime;

        private ActorBase target;

        public DamageInfo(ActorBase target)
        {
            this.target = target;
        }
        public void ShowDamage(int direction, DamageNumber effectObj)
        {
            //Create Damage Number:
            DamageNumber newDamageNumber = effectObj.Spawn(target.CenterPosition, Mathf.RoundToInt(damage));
            if (direction < 0)
            {
                newDamageNumber.lerpSettings.minX = -1f;
                newDamageNumber.lerpSettings.maxX = -0.5f;
            }
            else
            {
                newDamageNumber.lerpSettings.minX = 1f;
                newDamageNumber.lerpSettings.maxX = 0.5f;
            }
        }
        public void Clear()
        {
            damage = 0;
        }
    }
}