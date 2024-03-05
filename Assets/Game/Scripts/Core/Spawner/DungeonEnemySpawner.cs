using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using BansheeGz.BGDatabase;
using com.mec;
using Cysharp.Threading.Tasks;
using Engine;
using GameUtility;
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
    public class DungeonEnemySpawner : BaseMonsterSpawner
    {

        private List<Actor> m_ActorSpawned;
        private DungeonEntity m_DungeonEntity;
        private DungeonRoomTable m_RoomDatabase;
        private DungeonTable m_DungeonDatabase;

        private CancellationTokenSource m_Cts;
        private int m_CurrentWave = 0;
        public int CurrentWaveEnemy => m_CurrentWave;
        public int NextWaveEnemy => m_CurrentWave + 1;

        private List<CoroutineHandle> m_CoroutineHandleWave;

        public DungeonEnemySpawner(EnemyFactory monsterFactory, Bound2D spawnBound, TeamManager teamManager, DungeonEntity dungeonEntity, int currentWave) : base(monsterFactory, spawnBound, teamManager)
        {
            Messenger.AddListener<Actor>(EventKey.ActorDie, OnActorDie);

            m_Cts = new CancellationTokenSource();
            m_CoroutineHandleWave = new List<CoroutineHandle>();
            m_ActorSpawned = new List<Actor>();
            m_DungeonEntity = dungeonEntity;
            m_CurrentWave = currentWave;
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

        private void OnActorDie(Actor actor)
        {
            if (!m_ActorSpawned.Contains(actor))
            {
                return;
            }

            m_ActorSpawned.Remove(actor);
            if (actor.Tagger.HasTag(Tags.BossTag))
            {
                // Boss die => all enemies die
                foreach (var spawned in m_ActorSpawned)
                {
                    spawned.Health.Invincible = true;
                    spawned.Health.CurrentHealth = 0;
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

        public void NextWave()
        {
            m_CurrentWave++;
        }

        protected override IEnumerator<float> _Spawn(float delaySpawn)
        {
            int currentWave = CurrentWaveEnemy;
            yield return Timing.WaitForSeconds(delaySpawn);
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

                foreach (var position in listPositions)
                {
                    SpawnMonster(enemiesInf.Enemy, 1, position, m_Cts.Token).Forget();
                }
                yield return Timing.DeltaTime;
            }
        }
        protected override void ClearAll()
        {
            Logger.Log("Clear all actor");
        }
    }
}
