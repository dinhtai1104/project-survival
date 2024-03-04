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
using static UnityEditor.PlayerSettings;
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
        public int CurrentWave => m_CurrentWave;
        public int NextWave => m_CurrentWave + 1;

        private List<CoroutineHandle> m_CoroutineHandleWave;
        public DungeonEnemySpawner(EnemyFactory monsterFactory, Bound2D spawnBound, DungeonEntity dungeonEntity, int currentWave) : base(monsterFactory, spawnBound)
        {
            m_CoroutineHandleWave = new List<CoroutineHandle>();
            m_ActorSpawned = new List<Actor>();
            Messenger.AddListener<Actor>(EventKey.ActorDie, OnActorDie);
            m_DungeonEntity = dungeonEntity;
            m_CurrentWave = currentWave;
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

        protected override IEnumerator<float> _Spawn(float delaySpawn)
        {
            int currentWave = CurrentWave;
            yield return Timing.WaitForSeconds(delaySpawn);
            m_Cts = new CancellationTokenSource();
            // Spawn Wave
            var waveData = m_DungeonEntity.Waves[currentWave];
            foreach (var @event in waveData.EventEnemies)
            {
                var roomEvent = m_RoomDatabase.GetRoomTag(@event.TagRoom);
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
                    yield return Timing.DeltaTime;
                }
            }
        }
    }
}
