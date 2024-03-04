using com.mec;
using Cysharp.Threading.Tasks;
using Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class BaseMonsterSpawner
    {
        public event Action OnCompleteSpawning;
        public event Action<Actor> OnSpawning;
        private CoroutineHandle m_CoroutineHandle;

        private MonsterFactory m_MonsterFactory;
        private Bound2D m_SpawnBound;

        protected BaseMonsterSpawner(MonsterFactory monsterFactory, Bound2D spawnBound)
        {
            m_MonsterFactory = monsterFactory;
            m_SpawnBound = spawnBound;
        }

        public void StartSpawn(float delaySpawn)
        {
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

        public async UniTask<MonsterActor> SpawnMonster(int id, int monsterLevel, Vector2 position)
        {
            var monster = await m_MonsterFactory.CreateMonster(id, monsterLevel, position);
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
