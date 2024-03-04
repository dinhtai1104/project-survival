using com.mec;
using Cysharp.Threading.Tasks;
using Engine;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public abstract class BaseMonsterSpawner
    {
        public event Action OnCompleteSpawning;
        public event Action<Actor> OnSpawning;
        private CoroutineHandle m_CoroutineHandle;

        private EnemyFactory m_EnemyFactory;
        private Bound2D m_SpawnBound;

        public EnemyFactory EnemyFactory => m_EnemyFactory;
        public Bound2D Bound => m_SpawnBound;

        protected BaseMonsterSpawner(EnemyFactory monsterFactory, Bound2D spawnBound)
        {
            m_EnemyFactory = monsterFactory;
            m_SpawnBound = spawnBound;
        }

        public virtual void StartSpawn(float delaySpawn)
        {
            StopSpawn();
            m_CoroutineHandle = Timing.RunCoroutine(_Spawn(delaySpawn));
        }

        public void StopSpawn()
        {
            Timing.KillCoroutines(m_CoroutineHandle);
        }
        public void PauseSpawn()
        {
            Timing.PauseCoroutines(m_CoroutineHandle);
        }
        public void ResumeSpawn()
        {
            Timing.PauseCoroutines(m_CoroutineHandle);
        }

        protected abstract IEnumerator<float> _Spawn(float delaySpawn);

        public async UniTask<EnemyActor> SpawnMonster(string id, int monsterLevel, Vector2 position, CancellationToken token = default)
        {
            var monster = await m_EnemyFactory.CreateEnemy(id, monsterLevel, position, token);
            if (monster != null)
            {
                await UniTask.Yield();
                monster.Movement.SetBound(m_SpawnBound);
                monster.Init();

                OnSpawning?.Invoke(monster);

                AddToSpawnedActor(monster);
                return monster;
            }
            Logger.LogError("Null Monster: " + id);
            return null;
        }

        protected abstract void AddToSpawnedActor(Actor actor);
    }
}
