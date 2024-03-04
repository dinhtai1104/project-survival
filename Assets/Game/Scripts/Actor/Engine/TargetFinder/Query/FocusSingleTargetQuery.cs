using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public class FocusSingleTargetQuery : MonoBehaviour, ITargetQuery
    {
        [SerializeField] private bool m_UsingTagFilter;

        [SerializeField, ShowIf("$m_UsingTagFilter")]
        private string m_TagFilter;

        [SerializeField] private SOStorageActor m_ActiveEnemies;

        private Actor m_Actor;
        private Actor m_Target;

        public void Init(ITargetFinder finder)
        {
            m_Actor = finder.Owner;
        }

        public Actor GetTarget()
        {
            if (m_Target != null && m_Target.gameObject.activeInHierarchy)
            {
                return m_Target;
            }

            m_Target = FindEnemy(m_ActiveEnemies.Actors);
            return m_Target;
        }

        public void SetTarget(Actor target)
        {
            m_Target = target;
        }

        public void ForceUpdateTarget()
        {
        }

        public void OnUpdate()
        {
        }

        private Actor FindEnemy(IList<Actor> enemies)
        {
            Actor target = null;
            for (var i = 0; i < enemies.Count; ++i)
            {
                var enemyActor = enemies[i];
                if (enemyActor != null && enemyActor.gameObject.activeInHierarchy && !enemyActor.IsDead)
                {
                    if (m_UsingTagFilter && !enemyActor.CompareTag(m_TagFilter)) continue;
                    target = enemyActor;
                }
            }

            return target;
        }
    }
}