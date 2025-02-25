using System.Collections;
using System.Collections.Generic;
using ExtensionKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public class FocusSingleTargetQuery : MonoBehaviour, ITargetQuery
    {
        [SerializeField] private bool m_UsingTagFilter;

        [SerializeField, ShowIf("$m_UsingTagFilter")]
        private string m_TagFilter;


        private ActorBase m_Actor;
        private ActorBase m_Target;

        public void Init(ITargetFinder finder)
        {
            m_Actor = finder.Owner;
        }

        public ActorBase GetTarget(IList<ActorBase> targets)
        {
            if (m_Target != null && m_Target.gameObject.activeInHierarchy)
            {
                return m_Target;
            }

            m_Target = FindEnemy(targets);
            return m_Target;
        }

        public void SetTarget(ActorBase target)
        {
            m_Target = target;
        }

        public void ForceUpdateTarget()
        {
        }

        public void OnUpdate()
        {
        }

        private ActorBase FindEnemy(IList<ActorBase> enemies, params ActorBase[] except)
        {
            ActorBase target = null;
            for (var i = 0; i < enemies.Count; ++i)
            {
                var enemyActor = enemies[i];
                if (enemyActor != null && enemyActor.gameObject.activeInHierarchy && !enemyActor.IsDead)
                {
                    if (enemyActor.IsActivated == false) continue;
                    if (except.IsNotNull() && except.Contains(enemyActor)) continue;
                    if (m_UsingTagFilter && !enemyActor.CompareTag(m_TagFilter)) continue;
                    target = enemyActor;
                }
            }

            return target;
        }

        public ActorBase GetTarget(IList<ActorBase> targets, params ActorBase[] except)
        {
            if (m_Target != null && m_Target.gameObject.activeInHierarchy)
            {
                return m_Target;
            }

            m_Target = FindEnemy(targets, except);
            return m_Target;
        }
    }
}