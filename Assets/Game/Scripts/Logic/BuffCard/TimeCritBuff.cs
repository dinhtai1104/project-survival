using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor.Buff
{
    public class TimeCritBuff : AbstractBuff
    {
        public ValueConfigSearch CoolDown,Duration;
        private float coolDown, duration;
        public AssetReferenceGameObject effectRef;
        Effect.EffectAbstract effect;
        public bool isBuffed = false;
        private void OnEnable()
        {
            coolDown = CoolDown.FloatValue;
            duration = Duration.FloatValue;
            time = Time.time;
            Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
        }

        private void OnGameStart(int dungeon, int stage)
        {
            time = Time.time;
            isBuffed = false;
        }

        void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource dmg)
        {
            if (!isBuffed) return;
            if (attacker == Caster)
            {
                dmg.IsCrit = true;
            }
        }

        float time;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isBuffed)
            {
                if (Time.time - time >= duration)
                {
                    time = Time.time;
                    RemoveBuff();
                }
            }
            else
            {
                if (Time.time - time >= coolDown)
                {
                    time = Time.time;
                    BuffCrit();
                }
            }
        }

        void BuffCrit()
        {
            isBuffed = true;
            Game.Pool.GameObjectSpawner.Instance.Get(effectRef.RuntimeKey.ToString(), obj =>
            {
                effect = obj.GetComponent<Effect.EffectAbstract>();
                effect.Active(Caster.GetMidTransform().position).SetParent(Caster.GetTransform());
            });
        }
        void RemoveBuff()
        {
            isBuffed = false;
            effect.Stop();
            effect = null;
        }
        public override void Play()
        {
        }
       
    } 
}