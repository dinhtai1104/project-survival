using ExtensionKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class ClosestTargetQuery : MonoBehaviour, ITargetQuery
    {
        [SerializeField] private bool m_UsingTagFilter;
        [SerializeField] private string m_TagFilter;
        [SerializeField, Range(0f, 3f)] private float m_Cooldown;

        private ActorBase m_Actor;
        private ActorBase m_Target;
        private float m_Timer;
        private bool m_IsCooldown;

        public bool UsingTagFilter
        {
            set { m_UsingTagFilter = value; }
            get { return m_UsingTagFilter; }
        }

        public void Init(ITargetFinder finder)
        {
            m_Actor = finder.Owner;
        }

        public ActorBase GetTarget(IList<ActorBase> targets)
        {
            if (m_IsCooldown && m_Target != null && m_Target.gameObject.activeInHierarchy)
            {
                return m_Target;
            }

            m_IsCooldown = true;
            m_Target = FindClosestEnemy(targets);
            return m_Target;
        }

        public void SetTarget(ActorBase target)
        {
            m_Target = target;
        }

        public void ForceUpdateTarget()
        {
            m_Timer = 0f;
            m_IsCooldown = false;
            m_Target = null;
        }

        public void OnUpdate()
        {
            if (m_IsCooldown)
            {
                m_Timer += Time.deltaTime;

                if (m_Timer >= m_Cooldown)
                {
                    m_Timer = 0f;
                    m_IsCooldown = false;
                }
            }
        }

        private ActorBase FindClosestEnemy(IList<ActorBase> enemies, params ActorBase[] except)
        {
            float minDist = float.MaxValue;
            ActorBase target = null;

            for (int i = 0; i < enemies.Count; ++i)
            {
                ActorBase enemyActor = enemies[i];

                if (enemyActor != null && enemyActor.gameObject.activeInHierarchy && !enemyActor.IsDead)
                {
                    if (enemyActor.IsActivated == false) continue;
                    if (except.IsNotNull() && except.Contains(enemyActor)) continue;
                    if (m_UsingTagFilter && !enemyActor.CompareTag(m_TagFilter)) continue;

                    float dist = (enemyActor.BotPosition - m_Actor.BotPosition).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = enemyActor;
                    }
                }
            }

            return target;
        }

        public ActorBase GetTarget(IList<ActorBase> targets, params ActorBase[] except)
        {
            return FindClosestEnemy(targets, except);
        }
    }
}