using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public class SingleRaycastTargetQuery : MonoBehaviour, ITargetQuery
    {
        [SerializeField] private Vector3 m_Direction;
        [SerializeField] private float m_RayDistance = 1f;
        [SerializeField] private int m_TargetLimit = 1;
        [SerializeField] private float m_OriginOffetY = 0.2f;

        private ITargetFinder m_Finder;
        private Actor m_Target;
        private RaycastHit2D[] m_HitResults;

        public void Init(ITargetFinder finder)
        {
            m_Finder = finder;
            m_HitResults = new RaycastHit2D[m_TargetLimit];
            m_RayDistance = 10;
        }

        public Actor GetTarget()
        {
            if (m_Target != null && !m_Target.IsDead)
            {
                return m_Target;
            }

            m_Target = FindTarget();
            return m_Target;
        }

        public void SetTarget(Actor target)
        {
            m_Target = target;
        }

        public void OnUpdate()
        {
        }

        public void ForceUpdateTarget()
        {
            m_Target = null;
        }

        private Actor FindTarget()
        {
            var origin = m_Finder.Owner.Trans.position;
            origin.y += m_OriginOffetY;
#if UNITY_EDITOR
            Debug.DrawRay(origin, m_Direction * m_RayDistance, Color.green, 0.1f);
#endif
            var count = Physics2D.RaycastNonAlloc(origin, m_Direction, m_HitResults, m_RayDistance, m_Finder.Owner.EnemyLayerMask);
            if (count > 0)
            {
                var rayHit = m_HitResults[0].transform;
                m_Target = rayHit.GetComponent<Actor>();
                if (m_Target != null)
                {
                    return m_Target;
                }
            }

            return null;
        }
    }
}