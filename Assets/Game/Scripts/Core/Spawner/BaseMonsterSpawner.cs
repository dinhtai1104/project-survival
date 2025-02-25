using com.mec;
using Cysharp.Threading.Tasks;
using Engine;
using ExtensionKit;
using Gameplay;
using Manager;
using Pool;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public abstract class BaseMonsterSpawner
    {
        private TeamManager m_TeamManager;

        public delegate void CompleteSpawningDelegate();
        public delegate void SpawningDelegate(ActorBase actor);
        public delegate void StartSpawning();

        public CompleteSpawningDelegate OnCompleteSpawning;
        public SpawningDelegate OnSpawning;
        public StartSpawning OnStartSpawning;

        private CoroutineHandle m_CoroutineHandle;
        private BaseSceneManager m_SceneManager;


        private EnemyFactory m_EnemyFactory;
        private Bound2D m_SpawnBound;
        private Bound2D m_EnemyBound;

        public EnemyFactory EnemyFactory => m_EnemyFactory;
        public Bound2D Bound => m_SpawnBound;
        public Bound2D EnemyBound => m_EnemyBound;

        public BaseSceneManager SceneManager { get => m_SceneManager; set => m_SceneManager = value; }
        public TeamManager TeamManager { get => m_TeamManager; set => m_TeamManager = value; }

        protected BaseMonsterSpawner(EnemyFactory monsterFactory, Bound2D spawnBound, TeamManager teamManager, Bound2D enemyBound)
        {
            m_EnemyFactory = monsterFactory;
            m_SpawnBound = spawnBound;
            SceneManager = GameSceneManager.Instance;
            m_TeamManager = teamManager;
            m_EnemyBound = enemyBound;
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

        public async UniTask<EnemyActor> SpawnMonster(string id, int monsterLevel, Vector2 position, CancellationToken token = default, string source = null)
        {
            var effectPrefab = SceneManager.GetAsset<GameObject>("Effect_Spawn_Enemy");
            if (effectPrefab != null)
            {
                var ef = PoolFactory.Spawn(effectPrefab);
                ef.transform.position = position;
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            var monster = m_EnemyFactory.CreateEnemy(id, monsterLevel, position, token);
            if (monster != null)
            {
                monster.Init(TeamManager.GetTeamModel(ConstantValue.MonsterTeamId));
                monster.Movement.SetBound(m_SpawnBound);

                OnSpawning?.Invoke(monster);
                TeamManager.AddActor(ConstantValue.MonsterTeamId, monster);

                AddToSpawnedActor(monster);
                if (source.IsNullOrEmpty())
                {
                    monster.Shared.SetShared(SharedKey.SourceSpawn, source);
                }


                // apply skill cooldown
                var skillDefault = monster.SkillCaster.GetSkillById(0);
                if (skillDefault != null)
                {
                    skillDefault.SetManuallyCooldown(new Stat(monster.Stats.GetValue(StatKey.AttackSpeed)));
                }
                else
                {
                    Debug.Log("Enemy this " + id + " Has no skill default!");
                }

                return monster;
            }
            await UniTask.Yield();
            Logger.LogError("Null Monster: " + id);
            return null;
        }

        protected abstract void AddToSpawnedActor(ActorBase actor);
        public abstract void ClearAll();
        public abstract void Exit();
    }
}
