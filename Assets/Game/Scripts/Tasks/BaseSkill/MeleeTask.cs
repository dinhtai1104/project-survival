using System.Collections;
using System.Collections.Generic;
using Engine;
using ExtensionKit;
using Pool;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using Event = UnityEngine.Event;

public class MeleeTask : SkillTask
{
    [SerializeField] private GameObject m_HitEffect;
    [SerializeField] private string[] m_Animations;
    [SerializeField, Range(0f, 10f)] private float m_HitRange;
    [SerializeField, Range(0f, 10f)] private float m_HitRadius = 0.3f;
    [SerializeField, Range(0f, 10f)] private float m_OriginOffsetY = 0.2f;
    [SerializeField] private string m_EventName = "attack_tracking";
    [SerializeField] private int m_TargetNumber = 1;
    [SerializeField] private bool m_SyncTimeScale;
    [SerializeField] private UnityEvent m_OnAttack;
    [SerializeField] private UnityEvent m_OnHit;
    [SerializeField] private bool m_FacingTarget = true;

    [SerializeField] private DamageDealer m_DamageDealer;

    private RaycastHit2D[] m_Hits;
    private Spine.EventData m_AttackEventData;
    private string m_CurrentAnimation;
    private int m_CurrentAnimationIndex;

    protected override void Awake()
    {
        base.Awake();
        m_Hits = new RaycastHit2D[m_TargetNumber];
    }


    public override void Begin()
    {
        m_DamageDealer.Init(Caster.Stats);
        m_DamageDealer.DamageSource.Value = Caster.Stats.GetBaseValue(StatKey.Damage);
        m_AttackEventData = Caster.Animation.FindEvent(m_EventName);
        if (m_AttackEventData != null)
        {
            Caster.Animation.SubscribeEvent(OnAttack);
        }

        base.Begin();

        if (m_Animations != null && m_Animations.Length > 0)
        {
            m_CurrentAnimationIndex = 0;
            m_CurrentAnimation = m_Animations[m_CurrentAnimationIndex];
            Caster.Movement.IsMoving = false;
            if (m_FacingTarget && Caster.AI && Caster.TargetFinder.CurrentTarget != null)
            {
                Caster.Movement.LookAt(Caster.TargetFinder.CurrentTarget.Trans.position);
            }
        }
        else
        {
            if (m_FacingTarget && Caster.AI && Caster.TargetFinder.CurrentTarget != null)
            {
                Caster.Movement.LookAt(Caster.TargetFinder.CurrentTarget.Trans.position);
            }

            Attack();
            IsCompleted = true;
        }
    }

    public override void Run()
    {
        base.Run();
        var actorAnim = Caster.Animation;
        if (m_CurrentAnimation.IsNotNull())
        {
            actorAnim.Play(0, m_CurrentAnimation, false);
            if (actorAnim.IsCurrentAnimationComplete)
            {
                if (m_CurrentAnimationIndex >= m_Animations.Length - 1)
                {
                    IsCompleted = true;
                }
            }
        }
    }

    public override void End()
    {
        base.End();
        if (m_SyncTimeScale)
        {
            Caster.Animation.TimeScale = 1f;
        }
        if (m_AttackEventData != null)
        {
            Caster.Animation.UnsubcribeEvent(OnAttack);
        }
    }

    public override void Interrupt()
    {
        base.Interrupt();
        if (m_SyncTimeScale)
        {
            Caster.Animation.TimeScale = 1f;
        }
        if (m_AttackEventData != null)
        {
            Caster.Animation.UnsubcribeEvent(OnAttack);
        }
    }

    private void OnAttack(TrackEntry trackEntry, Spine.Event e)
    {
        if (!IsRunning || m_AttackEventData != e.Data || !Caster.Animation.IsPlaying(m_CurrentAnimation))
        {
            return;
        }

        //var target = Caster.TargetFinder.CurrentTarget;
        //if (target != null)
        //{
        //    if (m_DamageDealer != null)
        //    {
        //        HitResult hitResult = m_DamageDealer.DealDamage(Caster, target);

        //        if (hitResult.Success)
        //        {
        //            m_OnHit.Invoke();
        //            //GameCore.Event.Fire(this, HittedSkillEventArgs.Create(Caster, target, Skill != null ? Skill.Id : -999));
        //        }
        //    }

        //    return;
        //}

        Attack();
    }

    private void Attack()
    {
        m_OnAttack.Invoke();

        Vector3 hitOrigin = Caster.Trans.position;
        hitOrigin.y += m_OriginOffsetY;
        hitOrigin.x = Caster.transform.position.x + m_HitRadius * Caster.Movement.DirectionSign;
        float range = m_HitRange - m_HitRadius;

        for (int i = 0; i < m_Hits.Length; ++i)
        {
            m_Hits[i] = default(RaycastHit2D);
        }

        int count = Physics2D.CircleCastNonAlloc(hitOrigin, m_HitRadius, Caster.Movement.FacingDirection, m_Hits, range, Caster.EnemyLayerMask);

#if UNITY_EDITOR
        Debug.DrawRay(hitOrigin, Caster.Movement.FacingDirection * range, Color.red, 0.1f);
#endif
        if (count > 0)
        {
            foreach (var hit in m_Hits)
            {
                if (hit != default(RaycastHit2D))
                {
                    ActorBase target = hit.collider.GetComponent<ActorBase>();

                    if (target != null)
                    {
                        if (m_DamageDealer != null)
                        {
                            HitResult hitResult = m_DamageDealer.DealDamage(Caster, target);

                            if (hitResult.Success)
                            {
                                m_OnHit.Invoke();


                                if (m_HitEffect != null)
                                {
                                    Vector3 hitPoint = hit.point;
                                    GameObject effectGO =
                                        PoolFactory.Spawn(m_HitEffect, hitPoint, Quaternion.identity);
                                }

                                //GameCore.Event.Fire(this, HittedSkillEventArgs.Create(Caster, target, Skill != null ? Skill.Id : -999));
                            }
                        }
                    }
                }
            }
        }
    }
}
