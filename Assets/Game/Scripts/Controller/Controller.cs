using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using BayatGames.SaveGameFree;
using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
    public class Controller : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Clear Save")]
        static  void ClearSave()
        {
            PlayerPrefs.DeleteAll();
            SaveGame.DeleteAll();
        }
      
#endif
        public static Controller Instance;
        public GameConfig gameConfig;
  
        //public AssetReference []  gameModeDatas;
        public GameController gameController;
        public int playCount = 0;
        private DungeonSessionSave session;
        

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                Messenger.AddListener<GameController>(EventKey.GameLoaded, OnGameLoaded);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }
        private void OnDestroy()
        {
            Messenger.RemoveListener<GameController>(EventKey.GameLoaded, OnGameLoaded);
        }
        public async UniTask StartLevel(GameMode gameMode,int dungeonId,EDungeonEvent eventType=EDungeonEvent.None)
        {
            if (gameMode == GameMode.Normal)
            {
                Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.PlayDungeon);
            }
            Logger.Log("START LEVEL " + gameMode + " " + dungeonId);
            if (gameMode != GameMode.DungeonEvent)
            {
                session = DataManager.Save.DungeonSession;
                DataManager.Save.DungeonSession.JoinDungeon(dungeonId);
            }
            else
            {
                session = DataManager.Save.DungeonEventSession;
                ((DungeonEventSessionSave)(session)).SetDungeonEvent(eventType);
                DataManager.Save.DungeonEventSession.JoinDungeon(dungeonId);
                switch ((session as DungeonEventSessionSave).Type)
                {
                    case EDungeonEvent.Gold:
                        Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.PlayDungeonGold);
                        break;
                    case EDungeonEvent.Scroll:
                        Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.PlayDungeonScroll);
                        break;
                    case EDungeonEvent.Stone:
                        Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.PlayDungeonStone);
                        break;
                }
            }
            session.Mode = gameMode;
            Sound.Controller.Instance.soundData.PlayStartSFX();
            Sound.Controller.Instance.StopMusic();
            Game.Controller.Instance.playCount++;
            LoadingScreen loadingScreen = (await Addressables.InstantiateAsync("LoadingScreen")).GetComponent<LoadingScreen>();
            loadingScreen.Show();
       
            SceneLoader sceneLoader = new SceneLoader(SceneKey.GAME);
            sceneLoader.onLastSceneHidden += async (loader) =>
            {
                gameController?.Destroy();
                gameController = null;
            };
            sceneLoader.onScenePresented += async (loader) => 
            {
                //Logger.Log("hehe0");
                //Logger.Log((int)gameMode);
                //Logger.Log("hehe1");
                //Logger.Log(gameModes == null);
                //Logger.Log(gameModes.Length);
                //Logger.Log(gameModes[(int)gameMode] == null);
                //for(int i = 0; i < gameModes.Length; i++)
                //{
                //    Logger.Log(i+" => "+(gameModes[i] == null));

                //}
                //Logger.Log("hehe2");
                //Logger.Log(gameModes[(int)gameMode].gameController==null);
                //Logger.Log("hehe3");
                //Logger.Log(gameModes[(int)gameMode].gameController.IsValid());
                //Logger.Log("hehe4");
                Addressables.LoadAssetAsync<GameModeData>("GameModeData_" + gameMode).Completed+= (result) => 
                {
                    Addressables.InstantiateAsync(result.Result.gameController).Completed += Controller_Completed;

                };
                Logger.Log("hehe5");
            };
            sceneLoader.onSceneLoading += async (loader,progress) => 
            {
                loadingScreen.SetProgress(progress);
            };
            sceneLoader.onSceneLoaded += async (loader) => 
            {
                await sceneLoader.ActiveScene(); 
            };
        }

        private void Controller_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
        {
            obj.Result.SetActive(true);
        }

      
        public void OnGameLoaded(GameController gameController)
        {
            this.gameController = gameController;
            InitGame();

            async UniTaskVoid InitGame()
            {
                await gameController.SetPlaySession(session);
                await gameController.Initialize();
                LoadingScreen.Instance.Hide();
            }
        }

        public async UniTask LoadMenuScene()
        {
            LoadingScreen loadingScreen = (await Addressables.InstantiateAsync("LoadingScreen")).GetComponent<LoadingScreen>();
            loadingScreen.Show();


            SceneLoader sceneLoader = new SceneLoader(SceneKey.MENU);
            sceneLoader.onScenePresented += async (loader) =>
            {
                loadingScreen.Hide();
            };
            sceneLoader.onLastSceneHidden += async (loader) =>
            {
                gameController?.Destroy();
                gameController = null;
            };
            sceneLoader.onSceneLoading += async (loader,progress) =>
            {
                loadingScreen.SetProgress(progress);
            };
            sceneLoader.onSceneLoaded += async (loader) =>
            {
                await sceneLoader.ActiveScene();
            };
        }
    }
}