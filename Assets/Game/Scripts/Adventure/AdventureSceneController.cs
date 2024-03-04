using Assets.Game.Scripts.Core.Data.Database;
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

namespace Assets.Game.Scripts.Adventure
{
    public class AdventureSceneController : BaseGameplayScene
    {
        private Bound2D m_Bound2D;
        private AdventureMonsterSpawner m_Spawner;
        private AdventureEntity m_AdventureEntity;

        public Bound2D Bound => m_Bound2D;
        public AdventureMonsterSpawner Spawner => m_Spawner;
        public AdventureEntity AdventureEntity => m_AdventureEntity;

        protected override void OnEnter()
        {
            base.OnEnter();
            var monsterDatabase = DataManager.Base.MonsterTable;
            m_Bound2D = new Bound2D();

            var monsterFactory = new MonsterFactory(monsterDatabase);
            m_Spawner = new AdventureMonsterSpawner(monsterFactory, Bound);
        }

        public async override UniTask RequestAssets()
        {
            // Request map data
            if (!Architecture.Get<ShortTermMemoryService>().HasMemory<AdventureMapSelectionMemory>())
            {
                // Fallback if adventure map select null
                var adventureMapFallback = new AdventureMapSelectionMemory(DataManager.Base.Adventure.Get(1));
                Architecture.Get<ShortTermMemoryService>().Remember(adventureMapFallback);
            }
            var synchorousLoading = new List<UniTask>();


            var selectMapMemory = Architecture.Get<ShortTermMemoryService>().RetrieveMemory<AdventureMapSelectionMemory>();

            // Request monster
            foreach (var wave in AdventureEntity.Waves)
            {
                foreach (var waveInfo in wave.EnemyInfo)
                {
                    foreach (var monsterId in waveInfo.Enemies)
                    {
                        var monsterEntity = DataManager.Base.MonsterTable.Get(monsterId);
                        var task = RequestAsset<GameObject>(monsterId.ToString(), monsterEntity.Path);
                        synchorousLoading.Add(task);
                    }
                }
            }


            var mapPath = "Adventure/Map_" + AdventureEntity.MapId;
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
