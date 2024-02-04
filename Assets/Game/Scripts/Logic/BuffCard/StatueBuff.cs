using System;
using UnityEngine;

namespace Game.GameActor.Buff
{
    public class StatueBuff : AbstractBuff
    {
        private bool trackingMoving = false;
        private float time = 0;
        private StatModifier fireRateAdd;

        private void OnEnable()
        {
            Messenger.AddListener<ActorBase>(EventKey.ContinueMovement, OnContinueMove);
            Messenger.AddListener<ActorBase>(EventKey.StopMovement, OnStopMove);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase>(EventKey.ContinueMovement, OnContinueMove);
            Messenger.RemoveListener<ActorBase>(EventKey.StopMovement, OnStopMove);
        }

        private void OnStopMove(ActorBase source)
        {
            if (source == Caster)
            {
                trackingMoving = false;
            }
        }

        private void OnContinueMove(ActorBase source)
        {
            if (source == Caster)
            {
                trackingMoving = true;
                time = 0;
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (!trackingMoving)
            {
                time += Time.deltaTime;
                if (time >= 1)
                {
                    time = 0;
                    AddSpeedAttack();
                }
            }
            else
            {
                if (fireRateAdd.Value != 0)
                {
                    fireRateAdd.Value = 0;
                }
            }
        }

        private void AddSpeedAttack()
        {
            fireRateAdd.Value += GetValue(StatKey.MinValue);
            if (fireRateAdd.Value >= GetValue(StatKey.MaxValue))
            {
                fireRateAdd.Value = GetValue(StatKey.MaxValue);
            }
        }

        public override void Play()
        {
            fireRateAdd = new StatModifier(EStatMod.PercentAdd, 0);
            Caster.Stats.AddModifier(StatKey.FireRate, fireRateAdd, this);
        }
    }
}