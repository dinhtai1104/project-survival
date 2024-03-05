using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Framework;
using Gameplay;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Dungeon
{
    public class DungeonSceneController : BaseGameplayScene
    {
        private Bound2D m_Bound2D;
        private DungeonEnemySpawner m_Spawner;
        private DungeonEntity m_DungeonEntity;

        public Bound2D Bound => m_Bound2D;
        public DungeonEnemySpawner Spawner => m_Spawner;
        public DungeonEntity DungeonEntity => m_DungeonEntity;

        protected override void OnEnter()
        {
            base.OnEnter();
            var mapInstance = PoolManager.Instance.Spawn(GetRequestedAsset<GameObject>("map"), transform).GetComponent<Map>();

            var enemyDatabase = DataManager.Base.EnemyTable;
            m_Bound2D = mapInstance.MapBound;

            var monsterFactory = new EnemyFactory(enemyDatabase);
            var currentWave = 0;
            m_Spawner = new DungeonEnemySpawner(monsterFactory, Bound, TeamManager, m_DungeonEntity, currentWave);

            // Spawn Player
            
            m_Spawner.StartSpawn(2);
        }

        public async override UniTask RequestAssets()
        {
            // Request map data
            if (!Architecture.Get<ShortTermMemoryService>().HasMemory<DungeonMapSelectionMemory>())
            {
                // Fallback if adventure map select null
                var nullDungeon = DataManager.Base.Dungeon.CreateDungeon(0);


                var adventureMapFallback = new DungeonMapSelectionMemory(nullDungeon);
                Architecture.Get<ShortTermMemoryService>().Remember(adventureMapFallback);
            }
            var synchorousLoading = new List<UniTask>();


            var selectMapMemory = Architecture.Get<ShortTermMemoryService>().RetrieveMemory<DungeonMapSelectionMemory>();
            m_DungeonEntity = selectMapMemory.DungeonEntity;

            //Request monster
            foreach (var wave in DungeonEntity.Waves)
            {
                foreach (var waveInfo in wave.WaveInfo.EventEnemies)
                {
                    foreach (var enemy in waveInfo.Room.EventSpawn)
                    {
                        var monsterEntity = DataManager.Base.EnemyTable.Get(enemy.Enemy);
                        var task = RequestAsset<GameObject>(enemy.Enemy, monsterEntity.Path);
                        synchorousLoading.Add(task);
                    }
                }
            }


            var mapPath = "Map/Dungeon_" + DungeonEntity.DungeonId + ".prefab";
            var taskMap = RequestAsset<GameObject>("map", mapPath);
            synchorousLoading.Add(taskMap);
            synchorousLoading.Add(base.RequestAssets());

            await UniTask.WhenAll(synchorousLoading);
        }

        protected override bool CheckWinCondition()
        {
            return false;
        }

        protected override bool CheckLoseCondition()
        {
            return false;
        }

        protected override void OnWin()
        {
        }

        protected override void OnLose()
        {
        }
    }
}
