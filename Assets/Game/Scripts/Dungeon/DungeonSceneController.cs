using Assets.Game.Scripts.Core.Data.Database;
using Assets.Game.Scripts.Core.Data.Database.Dungeon;
using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Assets.Game.Scripts.Events;
using Assets.Game.Scripts.GameScene.Dungeon.Main;
using Assets.Game.Scripts.Manager;
using Assets.Game.Scripts.Objects.Loots;
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
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Assets.Game.Scripts.Dungeon
{
    public class DungeonSceneController : BaseGameplayScene
    {
        [SerializeField] private AssetReference m_EffectSpawnEnemy;
        [SerializeField] private DungeonEnemySpawner m_Spawner;

        // Variables in game
        private Bound2D m_Bound2D;
        private DungeonEntity m_DungeonEntity;
        private TickSystem m_TimeWaveSystem;
        private DungeonGameplaySessionSave m_DungeonSession;

        // Properties in game
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
            RemoveEvents();
            RemoveSystem();
            return base.Exit(reload);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Simulator.Instance.setTimeStep(0.25f);
            Simulator.Instance.setAgentDefaults(5, 15, 3, 3, 3f, 4.0f, new Vector2RVO(0.0f, 0.0f));

            IsBossDied = false;
            var mainPanel = ScenePresenter.GetPanel<UIDungeonMainPanel>();
            mainPanel.GetHealthPlayerBar().Init(MainPlayer);
            mainPanel.SetupControlView(MainPlayer);
            mainPanel.Show();

            // Create tick system
            PrepareSystem();
            // Prepare level
            PrepareMapLevel();
            //Setup Event
            SetupEvents();
            // Start First Wave
            StartWave();
        }

        private void SetupEvents()
        {
            m_Spawner.OnBossDie += OnBossDie;
            Architecture.Get<EventMgr>().Subscribe<LootEventArgs>(LootEventHandler);
        }

        private void RemoveEvents()
        {
            m_Spawner.OnBossDie -= OnBossDie;
            Architecture.Get<EventMgr>().Unsubscribe<LootEventArgs>(LootEventHandler);
        }

        private void RemoveSystem()
        {
            GameplayLevelUpHandler.Dispose();
            LootObjectUpdater.Instance.Dispose();
        }
        #region EVENT
        private void LootEventHandler(object sender, IEventArgs e)
        {
            var evt = e as LootEventArgs;
            switch (evt.Type)
            {
                case Enums.ELootObject.Pickle:
                    GameplayLevelUpHandler.AddExp(3);
                    break;
                case Enums.ELootObject.HpKit:
                    MainPlayer.Health.Healing(10);
                    break;
            }
        }

        private void OnBossDie(Engine.Actor boss)
        {
            IsBossDied = true;
        }

        private void OnWaveTimeEndComplete(long seconds)
        {
            WaveEndTime().Forget();

            // For optimize UniTask (Call UniTaskVoid)
            async UniTaskVoid WaveEndTime()
            {
                LootObjectUpdater.Instance.DestroyAll();
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

                // CheckLevelUp
                for (int i = 0; i < GameplayLevelUpHandler.GetLevelInWave(); i++)
                {
                    await ShowLevelUpPanel();
                }
                if (GameplayLevelUpHandler.GetLevelInWave() > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                }
                GameplayLevelUpHandler.Reset();


                // Show Shop Wave
                ShowShopWavePanel();
            }
        }

        #endregion
        private void PrepareSystem()
        {
            m_TimeWaveSystem = new TickSystem();
            var m_Expbar = ScenePresenter.GetPanel<UIDungeonMainPanel>().GetExpPlayerBar();
            GameplayLevelUpHandler.RegisterUI(m_Expbar);
            GameplayLevelUpHandler.StartTracking();

            var loot = new LootObjectUpdater();
        }

        private void PrepareMapLevel()
        {
            var mapInstance = PoolFactory.Spawn(GetRequestedAsset<GameObject>("map"), transform).GetComponent<Map>();

            var enemyDatabase = DataManager.Base.EnemyTable;
            m_Bound2D = mapInstance.MapBound;
            MainPlayer.Movement.SetBound(m_Bound2D);

            var monsterFactory = new EnemyFactory(enemyDatabase);
            var currentWave = 0;
            m_Spawner = new DungeonEnemySpawner(monsterFactory, mapInstance.SpawnBound, TeamManager, mapInstance.MapBound, m_DungeonEntity, currentWave);

            // Spawn Player
            CameraController.Instance.Follow(MainPlayer.transform, Vector3.zero);
            CameraController.Instance.SetBoundary(mapInstance.CameraBoundaries);
        }

        protected virtual void StartWave()
        {
            m_Spawner.StartSpawn(WaveDungeon.DelayStart);
            ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave(string.Format("Wave {0}/{1}", CurrentWave + 1, MaxWave), LengthWave, true).Forget();

            var timeThisWave = LengthWave;
            m_TimeWaveSystem.Start(timeThisWave, OnUpdateWaveTime, OnWaveTimeEndComplete);
            MainPlayer.Health.Invincible = false;
        }

        public virtual async UniTask StartNextWave()
        {
            var loading = SceneManager.CreateSceneTransition(SceneManager.CurrentSceneData.EnterTransitionPrefab);
            loading.gameObject.SetActive(true);

            if (!MainPlayer.WeaponHolder.IsMax())
            {
                var gunEntity = DataManager.Base.Weapon.GetRandom()[ERarity.Common];
                var weapon = WeaponFactory.CreateWeapon(gunEntity);

                MainPlayer.WeaponHolder.AddWeapon(weapon);
            }
            await UniTask.Delay(200);
            loading.StartTransition();
            while (!loading.IsDone)
            {
                await UniTask.Yield();
            }

            m_Spawner.StartNextWave(2);
            //CameraController.Instance.SetCameraSize(WaveDungeon.WaveEntity.CameraSize);

            MainPlayer.Health.CurrentHealth = MainPlayer.Health.MaxHealth;
            ScenePresenter.GetPanel<UIDungeonMainPanel>().ShowByTransitions();

            var timeThisWave = LengthWave;
            if (timeThisWave != 0)
            {
                m_TimeWaveSystem.Start(timeThisWave, OnUpdateWaveTime, OnWaveTimeEndComplete);
                ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave(string.Format("Wave {0}/{1}", CurrentWave + 1, MaxWave), LengthWave, true).Forget();
            }
            else
            {
                // Attack Boss
                ScenePresenter.GetPanel<UIDungeonMainPanel>().StartWave("Wave Boss!!", LengthWave).Forget();
            }
            MainPlayer.Health.Invincible = false;
        }

        private UniTask ShowLevelUpPanel()
        {
            bool isFinish = false;
            ScenePresenter.CreateAsync<UIDungeonLevelUpPanel>(AddressableName.UIDungeonLevelUpPanel)
                   .ContinueWith(panel =>
                   {
                       panel.Show();
                       panel.onBeforeDestroy += () => isFinish = true;
                   }).Forget();
            return UniTask.WaitUntil(()=>isFinish);
        }

        public void ShowShopWavePanel()
        {
            ScenePresenter.CreateAsync<UIDungeonShopWavePanel>(AddressableName.UIDungeonShopWavePanel)
                    .ContinueWith(panel =>
                    {
                        panel.onBeforeDestroy += () =>
                        {
                            StartNextWave().Forget();
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
            Simulator.Instance.doStep();
            LootObjectUpdater.Instance.OnUpdate();

#if DEVELOPMENT
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (!Spawner.IsLastWave)
                {
                    OnWaveTimeEndComplete(0);
                }
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                MainPlayer.Health.Invincible = !MainPlayer.Health.Invincible;
                Logger.Log("Player Invincible: " + MainPlayer.Health.Invincible, Color.green);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                GameplayLevelUpHandler.AddExp(3);
                Logger.Log("Player Invincible: " + MainPlayer.Health.Invincible, Color.green);
            }

#endif
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

            string path = m_EffectSpawnEnemy.RuntimeKey.ToString();
            var taskEffect = RequestAsset<GameObject>("Effect_Spawn_Enemy", path);
            synchorousLoading.Add(taskEffect);

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
