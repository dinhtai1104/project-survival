using Assets.Game.Scripts.Core.Data.Database.Dungeon;
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
        private DungeonEntity m_DungeonEntity;
        private DungeonRoomTable m_RoomDatabase;
        private DungeonTable m_DungeonDatabase;

        private CancellationTokenSource m_Cts;
        [SerializeField] private int m_CurrentWave = 0;
        public int CurrentWaveEnemy => m_CurrentWave;
        public int NextWaveEnemy => m_CurrentWave + 1;

        private List<CoroutineHandle> m_CoroutineHandleWave;

        public DungeonEnemySpawner(EnemyFactory monsterFactory, Bound2D spawnBound, TeamManager teamManager, DungeonEntity dungeonEntity, int currentWave) : base(monsterFactory, spawnBound, teamManager)
        {
            m_Cts = new CancellationTokenSource();
            m_CoroutineHandleWave = new List<CoroutineHandle>();
            m_ActorSpawned = new List<Actor>();
            m_DungeonEntity = dungeonEntity;
            m_CurrentWave = currentWave;
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
            foreach (var coroutine in m_CoroutineHandleWave)
            {
                if (coroutine.IsValid)
                {
                    Timing.KillCoroutines(coroutine);
                }
            }
            m_CoroutineHandleWave.Clear();
        }

        public override void PauseSpawn()
        {
            base.PauseSpawn(); 
            foreach (var coroutine in m_CoroutineHandleWave)
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
            foreach(var coroutine in m_CoroutineHandleWave)
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
            if (actor.Tagger.HasTag(Tags.BossTag))
            {
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

        }

        public override void StartSpawn(float delaySpawn)
        {
            base.StartSpawn(delaySpawn);
        }

        public void StartNextWave(float delaySpawn)
        {
            m_CurrentWave++;
            StartSpawn(delaySpawn);
        }

        protected override IEnumerator<float> _Spawn(float delaySpawn)
        {
            int currentWave = CurrentWaveEnemy;
            yield return Timing.WaitForSeconds(delaySpawn);
            OnStartSpawning?.Invoke();

            m_Cts = new CancellationTokenSource();
            // Spawn Wave
            var waveData = m_DungeonEntity.Waves[currentWave];
            foreach (var @event in waveData.WaveInfo.EventEnemies)
            {
                var roomEvent = @event.Room;
                var coroutine = Timing.RunCoroutine(_SpawnEvent(roomEvent, @event.Time));
                m_CoroutineHandleWave.Add(coroutine);
            }
        }

        private IEnumerator<float> _SpawnEvent(DungeonRoomEntity roomEvent, float time)
        {
            yield return Timing.WaitForSeconds(time);

            foreach (var enemiesInf in roomEvent.EventSpawn)
            {
                bool isCluster = enemiesInf.IsCluster;
                var radiusCluster = enemiesInf.SpawnRadius;
                var zoneRandomPos = Random.onUnitSphere * enemiesInf.IdZone * 3;
                var distanceEnemy = enemiesInf.MinSpace;
                var listPositions = GameUtility.GameUtility.GetRandomPositionWithoutOverlapping(zoneRandomPos
                        , Vector2.one * radiusCluster, distanceEnemy, enemiesInf.Amount);

                var spawnStartPoint = TeamManager.GetTeamModel(ConstantValue.PlayerTeamId).Allies[0].CenterPosition;
                var spawn = new List<Vector2>();
                for (int i = 0; i < enemiesInf.Amount; i++)
                {
                    var rdPos = MathUtils.RandomInsideCircle(spawnStartPoint, distanceEnemy, enemiesInf.Amount);
                }
                foreach (var position in listPositions)
                {
                    SpawnMonster(enemiesInf.Enemy, 1, position, m_Cts.Token).Forget();
                }
                yield return Timing.DeltaTime;
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
