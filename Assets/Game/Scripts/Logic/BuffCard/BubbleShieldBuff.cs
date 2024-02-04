using Game.GameActor;
using System;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class BubbleShieldBuff : AbstractBuff
    {
        public ValueConfigSearch bubbleShield_Cooldown = new ValueConfigSearch("Buff_BubbleShield_Cooldown");
        private float timeCtr = 0;
        private bool isActiveShield;
        [SerializeField] private BubbleShieldObject shield;
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforHit);
        }

        private void OnBeforHit(ActorBase attacker, ActorBase defender, DamageSource dmgSource)
        {
            if (isActiveShield)
            {
                if (defender == Caster)
                {
                    dmgSource.Value = 0;
                }
            }
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforHit);
        }
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isActiveShield)
            {
                ActiveShield();
            }
            else
            {
                CooldownActiveShield(dt);
            }
        }

        private void ActiveShield()
        {
            isActiveShield = true;
        }

        private void CooldownActiveShield(float dt)
        {
            timeCtr += dt;
            if (timeCtr >= bubbleShield_Cooldown.FloatValue)
            {
                var shieldIns = PoolManager.Instance.Spawn(shield, Caster.transform);
                shieldIns.SetCaster(Caster);
                shieldIns.SetBuff(BuffEntity.Type);
                shieldIns.SetDuration(GetValue(StatKey.Time));
                shieldIns.AutoDestroy.onComplete += CompletedBubbleEffect;
                isActiveShield = true;
            }
        }

        private void CompletedBubbleEffect()
        {
            isActiveShield = false;
            timeCtr = 0;
        }

        public override void Play()
        {
            timeCtr = bubbleShield_Cooldown.FloatValue - 1f;
        }
    }
}