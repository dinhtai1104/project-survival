using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Handler
{
    public class DamageNumberHandler : MonoBehaviour
    {
        //cooldown between get hit, time until reset damage to 0
        public float damageTextCoolDown = 0.1f;
        public Dictionary<ActorBase, DamageInfo> dict = new Dictionary<ActorBase, DamageInfo>();
        [ShowInInspector]
        public Dictionary<EDamage, DamageNumbersPro.DamageNumber> prefabs = new Dictionary<EDamage, DamageNumber>();
        private void OnEnable()
        {
            Messenger.AddListener<DamageSource, IDamageDealer>(EventKey.GetHit, OnGetHit);

            for (int i = 0; i < transform.childCount; i++)
            {
                prefabs.Add((EDamage)i, transform.GetChild(i).GetComponent<DamageNumber>());
            }
        }



        private void OnDisable()
        {
            Messenger.RemoveListener<DamageSource, IDamageDealer>(EventKey.GetHit, OnGetHit);

        }

        private void OnGetHit(DamageSource damageSource, IDamageDealer damageDealer)
        {
            if (!dict.ContainsKey(damageSource.Defender))
            {
                dict.Add(damageSource.Defender, new DamageInfo(damageSource.Defender));
            }
            DamageInfo damageInfo = dict[damageSource.Defender];

            if (Time.time - damageInfo.lastHitTime >= damageTextCoolDown)
            {
                damageInfo.Clear();
            }
            damageInfo.damage += damageSource.Value;
            damageInfo.lastHitTime = Time.time;

            int direction = damageSource.Attacker.GetMidTransform().position.x < damageSource.Defender.GetMidTransform().position.x ? 1 : -1;
            damageInfo.ShowDamage(direction, damageDealer, damageSource, prefabs[damageSource._damageType]);



        }
    }


    [System.Serializable]
    public class DamageInfo
    {
        public float damage;
        public float lastHitTime;

        private ActorBase target;

        public DamageInfo(ActorBase target)
        {
            this.target = target;
        }
        public void ShowDamage(int direction, IDamageDealer damageDealer, DamageSource dmgSource, DamageNumbersPro.DamageNumber effectObj)
        {
            //Create Damage Number:
            DamageNumber newDamageNumber = effectObj.Spawn(damageDealer == null ? target.GetMidTransform().position : damageDealer.GetDamagePosition(), ((int)damage));
            if (direction < 0)
            {
                newDamageNumber.lerpSettings.minX = -2f;
                newDamageNumber.lerpSettings.maxX = -1.5f;
            }
            else
            {
                newDamageNumber.lerpSettings.minX = 1.5f;
                newDamageNumber.lerpSettings.maxX = 2f;
            }
        }
        public void Clear()
        {
            damage = 0;
        }
    }
}