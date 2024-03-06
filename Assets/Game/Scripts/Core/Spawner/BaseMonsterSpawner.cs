using com.mec;
using Cysharp.Threading.Tasks;
using Engine;
using Gameplay;
using Manager;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public abstract class BaseMonsterSpawner
    {
        private TeamManager m_TeamManager;

        public event Action OnCompleteSpawning;
        public event Action<Actor> OnSpawning;
        private CoroutineHandle m_CoroutineHandle;
        private BaseSceneManager m_SceneManager;


        private EnemyFactory m_EnemyFactory;
        private Bound2D m_SpawnBound;

        public EnemyFactory EnemyFactory => m_EnemyFactory;
        public Bound2D Bound => m_SpawnBound;

        public BaseSceneManager SceneManager { get => m_SceneManager; set => m_SceneManager = value; }
        public TeamManager TeamManager { get => m_TeamManager; set => m_TeamManager = value; }

        protected BaseMonsterSpawner(EnemyFactory monsterFactory, Bound2D spawnBound, TeamManager teamManager)
        {
            m_EnemyFactory = monsterFactory;
            m_SpawnBound = spawnBound;
            SceneManager = GameSceneManager.Instance;
            m_TeamManager = teamManager;
        }

        public virtual void StartSpawn(float delaySpawn)
        {
            StopSpawn();
            m_CoroutineHandle = Timing.RunCoroutine(_Spawn(delaySpawn));
        }

        public virtual void StopSpawn()
        {
            Timing.KillCoroutines(m_CoroutineHandle);
        }
        public virtual void PauseSpawn()
        {
            Timing.PauseCoroutines(m_CoroutineHandle);
        }
        public virtual void ResumeSpawn()
        {
            Timing.PauseCoroutines(m_CoroutineHandle);
        }

        protected abstract IEnumerator<float> _Spawn(float delaySpawn);

        public async UniTask<EnemyActor> SpawnMonster(string id, int monsterLevel, Vector2 position, CancellationToken token = default)
        {
            var monster = m_EnemyFactory.CreateEnemy(id, monsterLevel, position, token);
            if (monster != null)
            {
                await UniTask.Yield();
                monster.Init(TeamManager.GetTeamModel(ConstantValue.MonsterTeamId));
                monster.Movement.SetBound(m_SpawnBound);

                OnSpawning?.Invoke(monster);
                TeamManager.AddActor(ConstantValue.MonsterTeamId, monster);

                AddToSpawnedActor(monster);
                return monster;
            }
            Logger.LogError("Null Monster: " + id);
            return null;
        }

        protected abstract void AddToSpawnedActor(Actor actor);
        protected abstract void ClearAll();
    }
}
