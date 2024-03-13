using com.mec;
using DG.Tweening;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Weapon
{
    public class WeaponMeleeTask : SkillTask
    {
        [SerializeField] private Transform m_PivotTrans;
        [SerializeField] private Transform m_WeaponTrans;

        [SerializeField] private AnimationCurve m_FirstMoveCurve;
        [SerializeField] private AnimationCurve m_FirstRotateCurve;
        [SerializeField] private AnimationCurve m_MoveToOrigin;
        [SerializeField] private AnimationCurve m_RotateToOrigin;

        [SerializeField] private float m_Distance = 2;
        [SerializeField] private float m_AngleRotate = 30;
        [SerializeField] private float m_Speed = 1;

        [SerializeField] private float m_TimeSwingMove = 0.5f;
        [SerializeField] private float m_TimeSwingMoveDelay = 0.5f;
        [SerializeField] private float m_TimeSwingRotate = 0.4f;
        [SerializeField] private float m_TimeSwingRotateDelay = 0.4f;
        [SerializeField] private float m_TimeSwingBack = 0.4f;

        private CoroutineHandle m_Handle;
        public override void Begin()
        {
            base.Begin();
            m_Distance = Caster.Stats.GetValue(StatKey.AttackRange);
            m_Speed = Caster.Stats.GetValue(StatKey.AttackSpeed) / Caster.Stats.GetBaseValue(StatKey.AttackSpeed);
            m_AngleRotate = -Caster.Stats.GetValue(StatKey.AngleZone, 0);
            m_Handle = Timing.RunCoroutine(_WeaponAttack(), Segment.FixedUpdate);
        }

        public override void End()
        {
            base.End();
            Timing.KillCoroutines(m_Handle);
        }

        private IEnumerator<float> _WeaponAttack()
        {
            var startPos = m_WeaponTrans.localPosition;
            var startRotate = Caster.transform.localEulerAngles;

            var dir = m_WeaponTrans.right;
            Debug.Log(dir);
            var dest = startPos + dir * m_Distance;
            Debug.Log(dest);

            float sign = 1;
            if (dir.x < 0)
            {
                sign = -1;
            }

            var eulerAngles = startRotate;
            eulerAngles.z += m_AngleRotate * sign;

            // MoveWeapon
            m_WeaponTrans.DOLocalMove(dest, m_TimeSwingMove * m_Speed).SetEase(m_FirstMoveCurve);
            yield return Timing.WaitForSeconds(m_TimeSwingMoveDelay * m_Speed);

            if (m_AngleRotate != 0)
            {
                m_PivotTrans.DOLocalRotate(eulerAngles, m_TimeSwingRotate * m_Speed).SetEase(m_FirstRotateCurve);
                yield return Timing.WaitForSeconds(m_TimeSwingRotateDelay * m_Speed);
            }

            m_WeaponTrans.DOLocalMove(startPos, m_TimeSwingBack * m_Speed).SetEase(m_MoveToOrigin);

            if (m_AngleRotate != 0)
            {
                m_PivotTrans.DOLocalRotate(startRotate, m_TimeSwingBack * m_Speed).SetEase(m_RotateToOrigin);
            }

            yield return Timing.WaitForSeconds(m_TimeSwingBack * m_Speed);
            IsCompleted = true;
        }
    }
}
