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
            var enemyDatabase = DataManager.Base.EntityTable;
            m_Bound2D = new Bound2D();

            var monsterFactory = new EnemyFactory(enemyDatabase);
            m_Spawner = new DungeonEnemySpawner(monsterFactory, Bound, m_DungeonEntity, 0);
        }

        public async override UniTask RequestAssets()
        {
            // Request map data
            if (!Architecture.Get<ShortTermMemoryService>().HasMemory<DungeonMapSelectionMemory>())
            {
                // Fallback if adventure map select null
                var adventureMapFallback = new DungeonMapSelectionMemory(new DungeonEntity());
                Architecture.Get<ShortTermMemoryService>().Remember(adventureMapFallback);
            }
            var synchorousLoading = new List<UniTask>();


            var selectMapMemory = Architecture.Get<ShortTermMemoryService>().RetrieveMemory<DungeonMapSelectionMemory>();
            m_DungeonEntity = selectMapMemory.DungeonEntity;

            // Request monster
            //foreach (var wave in DungeonEntity.Waves)
            //{
            //    foreach (var waveInfo in wave.EnemyInfo)
            //    {
            //        foreach (var monsterId in waveInfo.Enemies)
            //        {
            //            var monsterEntity = DataManager.Base.EntityTable.Get(monsterId);
            //            var task = RequestAsset<GameObject>(monsterId.ToString(), monsterEntity.Path);
            //            synchorousLoading.Add(task);
            //        }
            //    }
            //}


            var mapPath = "Adventure/Map_" + DungeonEntity.DungeonId;
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
