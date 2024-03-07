using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Assets.Game.Scripts.GameScene.Dungeon.Main;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Events;
using Framework;
using Gameplay;
using Gameplay.Data;
using Gameplay.Dungeon;
using Pool;
using RVO;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Game.Scripts.Dungeon
{
    public class DungeonSceneController : BaseGameplayScene
    {
        private Bound2D m_Bound2D;
        [SerializeField]
        private DungeonEnemySpawner m_Spawner;
        private DungeonEntity m_DungeonEntity;
        private TickSystem m_TimeWaveSystem;
        private DungeonGameplaySessionSave m_DungeonSession;

        public Bound2D Bound => m_Bound2D;
        public DungeonEnemySpawner Spawner => m_Spawner;
        public DungeonEntity DungeonEntity => m_DungeonEntity;
        public int CurrentWave => Spawner.CurrentWaveEnemy;
        public int MaxWave => DungeonEntity.Waves.Count;
        public int LengthWave => DungeonEntity.Waves[CurrentWave].Length;
        public bool IsWaveEnd => CurrentWave >= MaxWave - 1;
        public bool IsBossDied { get; private set; }
        public WaveInDungeonEntity WaveDungeon => DungeonEntity.Waves[CurrentWave];

        public override UniTask Exit(bool reload)
        {
            m_Spawner.OnBossDie -= OnBossDie;
            return base.Exit(reload);
        }
        protected override void OnEnter()
        {
            base.OnEnter();
            Simulator.Instance.setTimeStep(0.25f);
            Simulator.Instance.setAgentDefaults(5f, 20, 3, 3, 2.0f, 2.0f, new Vector2RVO(0.0f, 0.0f));

            IsBossDied = false;
            ScenePresenter.GetPanel<UIDungeonMainPanel>().GetHealthPlayerBar().Init(MainPlayer);
            ScenePresenter.GetPanel<UIDungeonMainPanel>().Show();

            // Create tick system
            PrepareSystem();


            // Prepare level
            PrepareMapLevel();
            m_Spawner.OnBossDie += OnBossDie;

            // Start First Wave
            StartWave();
        }

        private void OnBossDie(Engine.Actor boss)
        {
            IsBossDied = true;
        }

        private void PrepareSystem()
        {
            m_TimeWaveSystem = new TickSystem();
        }

        private void PrepareMapLevel()
        {
            var mapInstance = PoolManager.Instance.Spawn(GetRequestedAsset<GameObject>("map"), transform).GetComponent<Map>();

            var enemyDatabase = DataManager.Base.EnemyTable;
            m_Bound2D = mapInstance.MapBound;
            MainPlayer.Movement.SetBound(m_Bound2D);

            var monsterFactory = new EnemyFactory(enemyDatabase);
            var currentWave = 0;
            m_Spawner = new DungeonEnemySpawner(monsterFactory, mapInstance.SpawnBound, TeamManager, m_DungeonEntity, currentWave);

            // Spawn Player
            CameraController.Instance.Follow(MainPlayer.transform, Vector3.zero);
            CameraController.Instance.SetBoundary(mapInstance.CameraBoundaries);
        }

        protected virtual void StartWave()
        {
            m_Spawner.StartSpawn(WaveDungeon.DelayStart);
            ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave(string.Format("Wave {0}/{1}", CurrentWave + 1, MaxWave), LengthWave, true);

            var timeThisWave = LengthWave;
            m_TimeWaveSystem.Start(timeThisWave, OnUpdateWaveTime, OnWaveTimeEndComplete);
            MainPlayer.Health.Invincible = false;
        }

        public virtual void StartNextWave()
        {
            m_Spawner.StartNextWave(2);
            MainPlayer.Health.CurrentHealth = MainPlayer.Health.MaxHealth;
            ScenePresenter.GetPanel<UIDungeonMainPanel>().ShowByTransitions();

            var timeThisWave = LengthWave;
            if (timeThisWave != 0)
            {
                m_TimeWaveSystem.Start(timeThisWave, OnUpdateWaveTime, OnWaveTimeEndComplete);
                ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave(string.Format("Wave {0}/{1}", CurrentWave + 1, MaxWave), LengthWave, true);
            }
            else
            {
                // Attack Boss
                ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave("Wave Boss!!", LengthWave);
            }
            MainPlayer.Health.Invincible = false;
        }

        private async void OnWaveTimeEndComplete(long seconds)
        {
            m_TimeWaveSystem.Stop();
            m_Spawner.StopSpawn();
            MainPlayer.Health.Invincible = true;

            // Collect Pickle
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            m_Spawner.ClearAll();
            ScenePresenter.GetPanel<UIDungeonMainPanel>().HideByTransitions().Forget();
            ScenePresenter.GetPanel<UIDungeonMainPanel>().StopControl();
            
            // Screen Wave
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            bool isLevelUp = Random.value > 0.5f;
            if (isLevelUp)
            {
                ScenePresenter.CreateAsync<UIDungeonLevelUpPanel>(AddressableName.UIDungeonLevelUpPanel)
                    .ContinueWith(panel =>
                    {
                        panel.onClosed += ShowShopWavePanel;
                        panel.Show();
                    }).Forget();
                return;
            }
            ShowShopWavePanel();
        }

        public void ShowShopWavePanel()
        {
            ScenePresenter.CreateAsync<UIDungeonShopWavePanel>(AddressableName.UIDungeonShopWavePanel)
                    .ContinueWith(panel =>
                    {
                        panel.onClosed += () =>
                        {
                            StartNextWave();
                        };
                        panel.Show();
                    }).Forget();
        }

        private void OnUpdateWaveTime(long seconds)
        {
            ScenePresenter.GetPanel<UIDungeonMainPanel>().UpdateTimer(seconds);
        }


        public override void Execute()
        {
            base.Execute();
            if (EndGame) return;
            ScenePresenter.Execute();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                OnWaveTimeEndComplete(0);
            }
#endif
            Simulator.Instance.doStep();
        }

        public async override UniTask RequestAssets()
        {
            // Request Session
            m_DungeonSession = new DungeonGameplaySessionSave();
            // Request ui
            // Create panel ui
            var uiTask = ScenePresenter.CreateAsync<UIDungeonMainPanel>(AddressableName.UIDungeonMainPanel).AsUniTask();

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
                foreach (var eventInfo in wave.WaveEntity.EnemiesEvents)
                {
                    var monsterEntity = DataManager.Base.EnemyTable.Get(eventInfo.Enemy);
                    var task = RequestAsset<GameObject>(monsterEntity.Id, monsterEntity.Path);
                    synchorousLoading.Add(task);
                }

                var monsterDefaultEntity = DataManager.Base.EnemyTable.Get(wave.WaveEntity.DefaultEnemy);
                var taskDefault = RequestAsset<GameObject>(wave.WaveEntity.DefaultEnemy, monsterDefaultEntity.Path);
                synchorousLoading.Add(taskDefault);
            }


            var mapPath = "Map/Dungeon_" + DungeonEntity.DungeonId + ".prefab";
            var taskMap = RequestAsset<GameObject>("map", mapPath);
            synchorousLoading.Add(taskMap);
            synchorousLoading.Add(base.RequestAssets());
            synchorousLoading.Add(uiTask);

            await UniTask.WhenAll(synchorousLoading);
        }

        protected override bool CheckWinCondition()
        {
            if (!IsWaveEnd) return false;
            return IsBossDied;
        }

        protected override bool CheckLoseCondition()
        {
            return MainPlayer.IsDead;
        }

        protected override void OnWin()
        {
            m_DungeonSession.Result = Enums.EBattleResult.Win;
        }

        protected override void OnLose()
        {
            m_DungeonSession.Result = Enums.EBattleResult.Lose;
        }

        protected override void OnEndGame()
        {
            base.OnEndGame();

            m_Spawner.PauseSpawn();
            m_TimeWaveSystem.Stop();

            ScenePresenter.GetPanel<UIDungeonMainPanel>().StopControl();
            ScenePresenter.CreateAsync<UIDungeonEndGamePanel>(AddressableName.UIDungeonEndGamePanel)
                .ContinueWith(panel => panel.Show(m_DungeonSession)).Forget();

            m_Spawner.Exit();
        }
    }
}
