using com.mec;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Utilities.Test
{
    public class MeleeBehaviourTest : MonoBehaviour
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Swing Weapon
                Timing.RunCoroutine(_WeaponAttack(), Segment.Update);
            }
        }

        private IEnumerator<float> _WeaponAttack()
        {
            var startPos = m_WeaponTrans.position;
            var startRotate = transform.eulerAngles;

            var dir = m_WeaponTrans.right;
            Debug.Log(dir);
            var dest = m_WeaponTrans.position + dir * m_Distance;
            Debug.Log(dest);

            var eulerAngles = m_PivotTrans.eulerAngles;
            eulerAngles.z += m_AngleRotate;

            // MoveWeapon
            m_WeaponTrans.DOMove(dest, m_TimeSwingMove * m_Speed).SetEase(m_FirstMoveCurve);
            yield return Timing.WaitForSeconds(m_TimeSwingMoveDelay * m_Speed);

            if (m_AngleRotate != 0)
            {
                m_PivotTrans.DORotate(eulerAngles, m_TimeSwingRotate * m_Speed).SetEase(m_FirstRotateCurve);
                yield return Timing.WaitForSeconds(m_TimeSwingRotateDelay * m_Speed);
            }

            m_WeaponTrans.DOMove(startPos, m_TimeSwingBack * m_Speed).SetEase(m_MoveToOrigin);

            if (m_AngleRotate != 0)
            {
                m_PivotTrans.DORotate(startRotate, m_TimeSwingBack * m_Speed).SetEase(m_RotateToOrigin);
            }
            yield return 0f;
        }
    }
}
