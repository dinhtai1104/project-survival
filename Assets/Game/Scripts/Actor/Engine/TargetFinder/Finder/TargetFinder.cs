using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public class TargetFinder : SerializedMonoBehaviour, ITargetFinder
    {
        private IList<Actor> EmptyActors = new List<Actor>();

        [SerializeField, Range(0f, 3f)] private float m_ScanTargetPeriod = 0.2f;

        [TypeReferences.ClassExtends(typeof(ITargetQuery))]
        public TypeReferences.ClassTypeReference StartQuery;

        [HideInInspector] public List<ITargetQuery> Queries;

        public Actor Owner { get; private set; }
        public ITargetQuery CurrentQuery { get; private set; }
        [ShowInInspector, ReadOnly] public Actor CurrentTarget { get; set; }

        public bool IsUpdatingTarget
        {
            get { return m_IsUpdatingTarget; }
            set { m_IsUpdatingTarget = value; }
        }

        private Dictionary<Type, ITargetQuery> m_QueryMap;
        private bool m_IsUpdatingTarget;
        private float m_ScanTargetTimer;

        private IList<Actor> _enemies;
        private IList<Actor> _allies;

#if UNITY_EDITOR
        [SerializeField] private List<Actor> _editorEnemies;
        [SerializeField] private List<Actor> _editorAllies;
#endif

        public IList<Actor> Enemies
        {
            get { return _enemies ?? EmptyActors; }
        }

        public IList<Actor> Allies
        {
            get { return _allies ?? EmptyActors; }
        }


        public void Init(Actor actor)
        {
            Owner = actor;
            m_IsUpdatingTarget = true;
            m_ScanTargetTimer = m_ScanTargetPeriod;

            Queries = new List<ITargetQuery>(transform.GetComponents<ITargetQuery>());

            _allies = actor.TeamModel.Allies;
            _enemies = actor.TeamModel.Enemies;

            m_QueryMap = new Dictionary<Type, ITargetQuery>();
            foreach (var query in Queries)
            {
                query.Init(this);
                m_QueryMap.Add(query.GetType(), query);
            }

            if (StartQuery.Type == null)
            {
                var nullQuery = new NullTargetQuery();
                StartQuery = nullQuery.GetType();
                m_QueryMap.Add(StartQuery, nullQuery);
            }

            CurrentQuery = m_QueryMap[StartQuery];
        }

        public void OnUpdate()
        {
#if UNITY_EDITOR
            _editorAllies = Allies as List<Actor>;
            _editorEnemies = Enemies as List<Actor>;
#endif

            if (IsUpdatingTarget)
            {
                CurrentQuery?.OnUpdate();

                m_ScanTargetTimer += Time.deltaTime;
                if (m_ScanTargetTimer >= m_ScanTargetPeriod)
                {
                    m_ScanTargetTimer = 0f;
                    UpdateTarget();
                }

                if (CurrentTarget != null && (CurrentTarget.IsDead || CurrentTarget.Collider != null && CurrentTarget.Collider.enabled == false))
                {
                    ForceUpdateTarget();
                }

                for (int i = Enemies.Count - 1; i >= 0; i--)
                {
                    var enemy = Enemies[i];
                    if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    {
                        Enemies.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdateTarget()
        {
            CurrentTarget = CurrentQuery?.GetTarget(Enemies);
        }

        public void ForceUpdateTarget()
        {
            CurrentQuery?.ForceUpdateTarget();
        }

        public void Clear()
        {
            CurrentTarget = null;
            CurrentQuery?.ForceUpdateTarget();
        }

        public bool ChangeQuery(Type queryType)
        {
            if (CurrentQuery != null && CurrentQuery.GetType() == queryType)
            {
                Debug.LogWarning("@TargetFinder: Trying to change to Query of type: " + queryType +
                                 " but it's already the active Query.");
                return false;
            }

            if (m_QueryMap.ContainsKey(queryType))
            {
                CurrentQuery = m_QueryMap[queryType];
                return true;
            }

            Debug.LogWarning("@TargetFinder: Can't find QueryQuery of type: " + queryType);
            return false;
        }

        public bool ChangeQuery<TQuery>() where TQuery : ITargetQuery
        {
            return ChangeQuery(typeof(TQuery));
        }
    }
}