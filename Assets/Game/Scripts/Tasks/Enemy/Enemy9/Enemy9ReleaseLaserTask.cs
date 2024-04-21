using Damagable;
using Engine;
using ExtensionKit;
using Pool;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Enemy.Enemy9
{
    public class Enemy9ReleaseLaserTask : SkillTask
    {
        [SerializeField] private Transform m_Firepoint;
        [SerializeField] private string m_AnimationName;
        [SerializeField] private string m_AnimationEventName = "attack_tracking";

        [SerializeField] private BindConfig m_AngleLazer = new BindConfig("[0]AngleLazer", 25);
        [SerializeField] private BindConfig m_DamageLazer = new BindConfig("[0]DamageLazer", 1.1f);
        [SerializeField] private BindConfig m_IntervalLazer = new BindConfig("[0]IntervalLazer", 0.15f);
        [SerializeField] private BindConfig m_LengthLazer = new BindConfig("[0]LengthLazer", 4f);
        [SerializeField] private GameObject m_LazerPrefab;

        private DamageDealer m_DamageDealer;

        private EventData m_Event;
        protected override void Awake()
        {
            base.Awake();
            m_DamageDealer = new DamageDealer();
        }

        public override void Begin()
        {
            m_DamageDealer.Init(Caster.Stats);

            m_AngleLazer.SetId(Caster.name);
            m_DamageLazer.SetId(Caster.name);
            m_IntervalLazer.SetId(Caster.name);
            m_LengthLazer.SetId(Caster.name);

            base.Begin();
            Caster.Animation.SubscribeComplete(OnSkillAnimationCompleted);
            m_Event = Caster.Animation.FindEvent(m_AnimationEventName);

            if (m_Event != null)
            {
                Caster.Animation.SubscribeEvent(OnSkillAnimationEvent);
            }

            if (m_AnimationName.IsNotNullAndEmpty())
            {
                Caster.Animation.EnsurePlay(0, m_AnimationName, false);
            }
            else
            {
                Action();
                IsCompleted = true;
            }
        }

        public override void End()
        {
            base.End();
            Caster.Animation.UnsubcribeComplete(OnSkillAnimationCompleted);
            if (m_Event != null)
            {
                Caster.Animation.UnsubcribeEvent(OnSkillAnimationEvent);
            }
        }

        public override void Interrupt()
        {
            base.Interrupt();
            Caster.Animation.UnsubcribeComplete(OnSkillAnimationCompleted);
            if (m_Event != null)
            {
                Caster.Animation.UnsubcribeEvent(OnSkillAnimationEvent);
            }
        }
        private void OnSkillAnimationEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (trackEntry.TrackCompareAnimation(m_AnimationName))
            {
                if (e.Data == m_Event)
                {
                    Action();
                }
            }
        }

        private void OnSkillAnimationCompleted(TrackEntry trackEntry)
        {
            if (trackEntry.TrackCompareAnimation(m_AnimationName))
            {
                IsCompleted = true;
            }
        }

        private void Action()
        {
            var target = Caster.TargetFinder.CurrentTarget;
            if (target == null) return;
            var dir = target.CenterPosition - Caster.CenterPosition;
            var angle = Mathf.Atan2(dir.y, dir.x);
            ReleaseLazer(angle - m_AngleLazer.FloatValue);
            ReleaseLazer(angle + m_AngleLazer.FloatValue);
        }

        private void ReleaseLazer(float angle)
        {
            var lazer = PoolFactory.Spawn(m_LazerPrefab, m_Firepoint.position, Quaternion.Euler(0, 0, angle)).GetComponent<Lazer2D>();
            lazer.DamageDealer?.CopyData(m_DamageDealer);
            lazer.TargetLayer = Caster.EnemyLayerMask;
            lazer.DamageDealer.DamageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, m_DamageLazer.FloatValue - 1));
            lazer.Owner = Caster;
            lazer.DamageInterval = m_IntervalLazer.FloatValue;
            lazer.LazerLength = m_LengthLazer.FloatValue;
            lazer.StartLazer();
        }
    }
}
