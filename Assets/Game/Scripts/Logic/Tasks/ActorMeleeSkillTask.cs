using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ActorMeleeSkillTask : SkillTask
{
    public string m_Animation;
    public string m_Vfx;


    public ValueConfigSearch m_AttackDmg;

    public Transform m_PosAttack;

    public LayerMask EnemyLayer;
    [SerializeField, Range(0f, 10f)] private float m_HitRange;
    [SerializeField, Range(0f, 10f)] private float m_HitRadius = 0.3f;
    [SerializeField, Range(0f, 10f)] private float m_OriginOffsetY = 0.2f;

    [SerializeField] private UnityEvent m_OnAttack;
    [SerializeField] private UnityEvent m_OnHit;
    [SerializeField] private bool m_FacingTarget = true;

    private RaycastHit2D[] m_Hits = new RaycastHit2D[5];

    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
        Caster.AnimationHandler.SetAnimation(m_Animation, false);
    }


    public override async UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        await base.End();
    }
    public override void OnStop()
    {
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        base.OnStop();
    }

    private void AnimationHandler_onCompleteTracking(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == m_Animation)
        {
            IsCompleted = true;
        }
    }
    private void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == m_Animation)
        {
            if (e.Data.Name == "attack_tracking")
            {
                OnAttack();
            }
        }
    }

    private void OnAttack()
    {
        m_OnAttack?.Invoke();

        Vector3 hitOrigin = m_PosAttack.position;
        hitOrigin.y += m_OriginOffsetY;
        hitOrigin.x = m_PosAttack.position.x + m_HitRadius * Caster.GetLookDirection().x;
        float range = m_HitRange - m_HitRadius;

        for (int i = 0; i < m_Hits.Length; ++i)
        {
            m_Hits[i] = default(RaycastHit2D);
        }

        int count = Physics2D.CircleCastNonAlloc(hitOrigin, m_HitRadius, Caster.GetLookDirection(), m_Hits, range, EnemyLayer);

#if UNITY_EDITOR
        Debug.DrawRay(hitOrigin, Caster.GetLookDirection() * range, Color.red, 0.1f);
#endif
        if (count > 0)
        {
            foreach (var hit in m_Hits)
            {
                if (hit != default(RaycastHit2D))
                {
                    ActorBase target = hit.collider.GetComponentInParent<ActorBase>();
                    if (target != null)
                    {
                        if (target.GetCharacterType() == ECharacterType.Player)
                        {
                            var dmgSource = new DamageSource
                            {
                                Attacker = Caster,
                                Defender = target as ActorBase,
                                _damage = new Stat(Caster.GetStatValue(StatKey.Dmg) * m_AttackDmg.FloatValue),
                            };
                            dmgSource._damageSource = EDamageSource.Weapon;

                            target.GetHit(dmgSource, target as ActorBase);
                            m_OnHit.Invoke();
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 hitOrigin = m_PosAttack.position;
        hitOrigin.y += m_OriginOffsetY;
        hitOrigin.x = m_PosAttack.position.x + m_HitRadius * Vector3.right.x;
        float range = m_HitRange - m_HitRadius;

        for (int i = 0; i < m_Hits.Length; ++i)
        {
            m_Hits[i] = default(RaycastHit2D);
        }

        int count = Physics2D.CircleCastNonAlloc(hitOrigin, m_HitRadius, Vector3.right, m_Hits, range, EnemyLayer);

        Gizmos.DrawWireSphere(hitOrigin, m_HitRadius);
    }
}