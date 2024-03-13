using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.Data.Database.Dungeon.EnemyEvent;
using BansheeGz.BGDatabase;
using com.mec;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Events;
using GameUtility;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework
{
    [System.Serializable]
    public class DungeonEnemySpawner : BaseMonsterSpawner
    {
        public delegate void BossDieDelegate(Actor boss);
        public BossDieDelegate OnBossDie;

        [SerializeField] private List<Actor> m_ActorSpawned;
        private Dictionary<string, int> m_EnemyAmountEachType;
        private Dictionary<string, int> m_EnemyTypeCap;

        private DungeonEntity m_DungeonEntity;
        private DungeonTable m_DungeonDatabase;

        private CancellationTokenSource m_Cts;
        [SerializeField] private int m_CurrentWave = 0;
        public int CurrentWaveEnemy => m_CurrentWave;
        public int NextWaveEnemy => m_CurrentWave + 1;
        public bool IsLastWave => CurrentWaveEnemy >= m_DungeonEntity.Waves.Count - 1;
        public WaveInDungeonEntity CurrentWave => m_DungeonEntity.Waves[CurrentWaveEnemy];

        private List<CoroutineHandle> m_CoroutineHandlSpawn;

        public DungeonEnemySpawner(EnemyFactory monsterFactory, Bound2D spawnBound, TeamManager teamManager, Bound2D movementBound, DungeonEntity dungeonEntity, int currentWave) : base(monsterFactory, spawnBound, teamManager, movementBound)
        {
            m_Cts = new CancellationTokenSource();
            m_CoroutineHandlSpawn = new List<CoroutineHandle>();
            m_ActorSpawned = new List<Actor>();
            m_DungeonEntity = dungeonEntity;
            m_CurrentWave = currentWave;
            m_EnemyAmountEachType = new Dictionary<string, int>();
            m_EnemyTypeCap = new Dictionary<string, int>();

            Architecture.Get<EventMgr>().Subscribe<ActorDieEventArgs>(ActorDieEvent);
        }

        public override void Exit()
        {
            Architecture.Get<EventMgr>().Unsubscribe<ActorDieEventArgs>(ActorDieEvent);
        }

        public override void StopSpawn()
        {
            base.StopSpawn();
            m_Cts?.Cancel();
            foreach (var coroutine in m_CoroutineHandlSpawn)
            {
                if (coroutine.IsValid)
                {
                    Timing.KillCoroutines(coroutine);
                }
            }
            m_CoroutineHandlSpawn.Clear();
        }

        public override void PauseSpawn()
        {
            base.PauseSpawn(); 
            foreach (var coroutine in m_CoroutineHandlSpawn)
            {
                if (coroutine.IsValid)
                {
                    Timing.PauseCoroutines(coroutine);
                }
            }
        }
        public override void ResumeSpawn()
        {
            base.ResumeSpawn();
            foreach(var coroutine in m_CoroutineHandlSpawn)
            {
                if (coroutine.IsValid)
                {
                    Timing.ResumeCoroutines(coroutine);
                }
            }
        }

        private void ActorDieEvent(object sender, IEventArgs e)
        {
            var @evt = e as ActorDieEventArgs;
            var actor = evt.m_Actor;

            if (!m_ActorSpawned.Contains(actor))
            {
                return;
            }

            m_ActorSpawned.Remove(actor);
            TeamManager.RemoveActor(ConstantValue.MonsterTeamId, actor);

            if (actor.Shared.HasShared(SharedKey.SourceSpawn))
            {
                var sourceSpawn = actor.Shared.GetShared<string>(SharedKey.SourceSpawn);
                try
                {
                    m_EnemyAmountEachType[sourceSpawn]--;
                    m_EnemyAmountEachType[sourceSpawn] = Mathf.Max(0, m_EnemyAmountEachType[sourceSpawn]);
                }
                catch
                {

                }
            }

            if (actor.Tagger.HasTag(Tags.BossTag))
            {
                StopSpawn();
                OnBossDie?.Invoke(actor);
                // Boss die => all enemies die
                foreach (var spawned in m_ActorSpawned)
                {
                    spawned.Health.Invincible = true;
                    spawned.Health.CurrentHealth = 0;
                    TeamManager.RemoveActor(ConstantValue.MonsterTeamId, spawned);
                }
            }
        }

        protected override void AddToSpawnedActor(Actor actor)
        {
            m_ActorSpawned.Add(actor);
            if (actor.Shared.HasShared(SharedKey.SourceSpawn))
            {
                var source = actor.Shared.GetShared<string>(SharedKey.SourceSpawn);
                if (m_EnemyAmountEachType.ContainsKey(source))
                {
                    m_EnemyAmountEachType[source]++;
                }
            }
        }

        public override void StartSpawn(float delaySpawn)
        {
            m_Cts = new CancellationTokenSource();
            base.StartSpawn(delaySpawn);
            m_EnemyAmountEachType.Clear();
            m_EnemyTypeCap.Clear();

            foreach (var evt in CurrentWave.WaveEntity.EnemiesEvents)
            {
                if (!m_EnemyTypeCap.ContainsKey(evt.Enemy))
                {
                    m_EnemyTypeCap.Add(evt.EventId + "_" + evt.Enemy, evt.Max);
                    m_EnemyAmountEachType.Add(evt.EventId + "_" + evt.Enemy, 0);
                }
            }
        }

        public void StartNextWave(float delaySpawn)
        {
            m_CurrentWave++;
            StartSpawn(delaySpawn);
        }

        private bool HasSpawnEnemyByCap(string enemyId)
        {
            if (m_EnemyTypeCap.ContainsKey(enemyId))
            {
                if (m_EnemyAmountEachType[enemyId] >= m_EnemyTypeCap[enemyId])
                {
                    return false;
                }
            }
            return true;
        }

        private bool HasSpawnEnemy(out int left)
        {
            var currentAmount = m_ActorSpawned.Count;
            var maxSpawn = CurrentWave.WaveEntity.MaxInMap;

            left = Mathf.Max(0, maxSpawn - currentAmount);

            return left > 0;
        }

        // Spawn highest floor
        protected override IEnumerator<float> _Spawn(float delaySpawn)
        {
            int currentWave = CurrentWaveEnemy;
            yield return Timing.WaitForSeconds(delaySpawn);
            OnStartSpawning?.Invoke();

            // highest Wave - Default Enemies
            var highestWave = Timing.RunCoroutine(_SpawnDefaultEnemies(), Segment.FixedUpdate);
            m_CoroutineHandlSpawn.Add(highestWave);

            // handle stage event enemies spawn
            var lowerWave = Timing.RunCoroutine(_SpawnStageEventEnemies(), Segment.FixedUpdate);
            m_CoroutineHandlSpawn.Add(lowerWave);
        }

        private IEnumerator<float> _SpawnStageEventEnemies()
        {
            yield return 0f;

            foreach (var @event in CurrentWave.WaveEntity.EnemiesEvents)
            {
                // spawn enemies event
                var lowestWave = Timing.RunCoroutine(_SpawnEnemiesEvent(@event), Segment.FixedUpdate);
                m_CoroutineHandlSpawn.Add(lowestWave);
            }
        }

        private IEnumerator<float> _SpawnEnemiesEvent(EnemiesEventEntity @event)
        {
            yield return 0f;
            float defaultCooldown = @event.Frequency;
            float currentTimer = 0;

            while (true)
            {
                if (currentTimer >= defaultCooldown)
                {
                    currentTimer = 0;
                    // Spawn
                    if (HasSpawnEnemy(out var left))
                    {
                        var amount = Mathf.Min(left, @event.Max);
                        var IsCluster = MathUtils.RollChance(@event.Cluster / 100f);
                        var centerCluster = MathUtils.RandomPointInBound2D(Bound);
                        var clusterPoint = MathUtils.GetPositionsInCircle(centerCluster, ConstantValue.ClusterRange, amount);

                        for (int i = 0; i < amount; i++)
                        {
                            if (HasSpawnEnemyByCap(@event.EventId + "_" + @event.Enemy) && MathUtils.RollChance(@event.Chance / 100f))
                            {
                                Vector2 rdPoint = MathUtils.RandomPointInBound2D(Bound);
                                if (IsCluster)
                                {
                                    rdPoint = clusterPoint[i];
                                }

                                SpawnMonster(@event.Enemy, CurrentWave.LevelEnemy, rdPoint, m_Cts.Token, source: @event.EventId + "_" + @event.Enemy).Forget();
                            }
                        }
                    }
                }

                currentTimer += Time.fixedDeltaTime;
                yield return 0f;
            }
        }

        private IEnumerator<float> _SpawnDefaultEnemies()
        {
            yield return 0f;
            float defaultCooldown = CurrentWave.Frequency;
            float currentTimer = 0;
            var waveDefault = CurrentWave.WaveEntity;
            bool isSpawned = false;
            while (true)
            {

                if (currentTimer >= defaultCooldown)
                {
                    currentTimer = 0;
                    // Spawn
                    if (HasSpawnEnemy(out var left) && MathUtils.RollChance(waveDefault.DefaultChance / 100f))
                    {
                        var amount = Mathf.Min(left, (int)(waveDefault.DefaultAmount * (1 + CurrentWave.RandomAdd)));
                        if (IsLastWave)
                        {
                            amount = 1;
                        }

                        var IsCluster = MathUtils.RollChance(waveDefault.DefaultCluster / 100f);
                        var centerCluster = MathUtils.RandomPointInBound2D(Bound);
                        var clusterPoint = MathUtils.GetPositionsInCircle(centerCluster, ConstantValue.ClusterRange, amount);

                        for (int i = 0; i < amount; i++)
                        {
                            if (HasSpawnEnemyByCap(waveDefault.DefaultEnemy))
                            {
                                Vector2 rdPoint = MathUtils.RandomPointInBound2D(Bound);
                                if (IsCluster)
                                {
                                    rdPoint = clusterPoint[i];
                                }
                                isSpawned = true;
                                SpawnMonster(waveDefault.DefaultEnemy, CurrentWave.LevelEnemy, rdPoint, m_Cts.Token).Forget();
                                
                            }
                        }
                    }
                }

                if (IsLastWave && isSpawned)
                {
                    break;
                }
                currentTimer += Time.fixedDeltaTime;
                yield return 0f;
            }
        }

        public override void ClearAll()
        {
            Logger.Log("Clear all actor");
            foreach (var actor in m_ActorSpawned)
            {
                actor.Health.Invincible = false;
                actor.Health.CurrentHealth = 0;
                TeamManager.RemoveActor(ConstantValue.MonsterTeamId, actor);
            }
            m_ActorSpawned.Clear();
        }
    }
}
