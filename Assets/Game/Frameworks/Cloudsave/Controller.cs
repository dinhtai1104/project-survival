using Assets.Game.Scripts.DungeonWorld.Save;
using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CloudSave
{
    [System.Serializable]
    public class SavePackage
    {
        public string id;
        public string timeStamp;
        public long coin,gem,xp;
        public int level;
        public string data;

        public SavePackage()
        {
        }
        public override string ToString()
        {
            return $"{id}: {timeStamp} \n{data}";
        }
    }
    public enum ESaveStatus
    {
        Success,Failed,Pending,Conflict
    }
    public enum EStatus
    {
        Normal,Conflict,Failed
    }
    public enum ESaveType
    {
        Local, Cloud
    }
    public class Controller : MonoBehaviour
    {
        public static Controller Instance;
        private const string secret = "41dfjd913f139r", salt = "101110411011";
        private const string ID = "id";
        private const string TIME = "timestamp";
        private const string DATA = "data";
        private const string GOLD = "coin";
        private const string GEM = "gem";
        private const string XP = "xp";
        private const string LEVEL = "level";
        private const string CollectionId = "UserData";
        private const string UDID = "UDID";
       
        private SavePackage savePackage=new SavePackage();

        private SavePackage cloudPackage = new SavePackage();
        private Dictionary<string, string> data = new Dictionary<string, string>();

        public SavePackage Local=> savePackage;
        public SavePackage Cloud=> cloudPackage;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

            }

        }
        private string GetID()
        {
            if (!PlayerPrefs.HasKey(UDID))
            {
                PlayerPrefs.SetString(UDID, System.Guid.NewGuid().ToString());
            }
            return PlayerPrefs.GetString(UDID);
        }

        public async UniTask ValidateAndSave()
        {
            if (FirebaseAuthentication.Instance.User==null ||( FirebaseAuthentication.Instance.User != null && FirebaseAuthentication.Instance.User.IsAnonymous)) return;
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            var validateStatus = await Validate();
            if (validateStatus == EStatus.Normal)
            {
                await Save();
            }
        }
        public async UniTask<EStatus> Validate()
        {
            EStatus result = EStatus.Normal;
            //prepare local save
            UpdateLocal();
            cloudPackage = await FetchMetadata();
            //if cloud save is not from this device 
            //show compare saves
            bool check = false;
            if (!string.IsNullOrEmpty(cloudPackage.id) && !cloudPackage.id.Equals(savePackage.id))
            {
                UI.PanelManager.CreateAsync<UIResolveSaveConflictPanel>(AddressableName.UIResolveSaveConflictPanel).ContinueWith(panel =>
                {
                    try
                    {
                        panel.SetUp(Local, Cloud);
                        panel.onResolve += (saveType, save) =>
                        {
                            WaitingPanel.Show(UniTask.Action(async () =>
                            {
                                Logger.Log("SELECT: " + saveType + " " + save.timeStamp);
                                save.id = GetID();
                                switch (saveType)
                                {
                                    case ESaveType.Local:
                                        await Push(save);
                                        break;
                                    case ESaveType.Cloud:
                                        //fetch cloud data first
                                        save = await FetchData();
                                        //if save data is OK
                                        if (!string.IsNullOrEmpty(save.data))
                                        {
                                            //push to cloud
                                            await Push(save);

                                            //OverwriteCurrentUserData(Newtonsoft.Json.JsonConvert.DeserializeObject<UserDataCloud>(
                                            //        new Zitga.Core.Toolkit.Compression.GZipAlgorithm().Decompress(new Zitga.Core.Toolkit.Encryption.AesCbcAlgorithm(secret, salt).Decrypt(save.data)),
                                            //        new ObscuredValueConverter()));
                                            try
                                            {
                                                OverwriteCurrentUserData(Newtonsoft.Json.JsonConvert.DeserializeObject<UserDataCloud>(
                                                    save.data,
                                                        new ObscuredValueConverter()));
                                            }
                                            catch (System.Exception e)
                                            {
                                                Logger.LogError(e);
                                                UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
                                                {
                                                    panel.SetUp("Save file from cloud is wrong. Try local save");
                                                }).Forget();
                                                result = EStatus.Failed;
                                            }
                                        }
                                        else
                                        {
                                            UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
                                            {
                                                panel.SetUp("Save file from cloud is wrong. Try local save");
                                            }).Forget();
                                            result = EStatus.Failed;

                                        }

                                        break;
                                }
                                UpdateLocal();
                                WaitingPanel.Hide();
                                check = true;

                            }));

                        };
                    }
                    catch(System.Exception e)
                    {
                        UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
                        {
                            panel.SetUp(e.Message+" "+e.InnerException);
                        }).Forget();
                    }
                }).Forget();
                await UniTask.WaitUntil(() => check);
                return result;
            }
            else
            {
                return result;
            }
          

        }
        public void UpdateLocal()
        {
            savePackage.id = GetID();
            savePackage.timeStamp = UnbiasedTime.Instance.Now().ToString();
            //savePackage.data = JsonUtility.ToJson(NewUserDataCloud());
            savePackage.data = Newtonsoft.Json.JsonConvert.SerializeObject(NewUserDataCloud(),new ObscuredValueConverter());
            savePackage.coin = (long)DataManager.Save.Resources.GetResource(EResource.Gold);
            savePackage.gem = (long)DataManager.Save.Resources.GetResource(EResource.Gem);
            savePackage.level = GameSceneManager.Instance.PlayerData.ExpHandler.CurrentLevel;
            savePackage.xp = GameSceneManager.Instance.PlayerData.ExpHandler.TotalExpAmount;
        }
        public async UniTask Push(SavePackage savePackage)
        {
            data.Clear();
            data.Add(ID, savePackage.id);
            data.Add(TIME, savePackage.timeStamp.ToString());
            data.Add(GOLD, savePackage.coin.ToString());
            data.Add(GEM, savePackage.gem.ToString());
            data.Add(LEVEL, savePackage.level.ToString());
            data.Add(XP, savePackage.xp.ToString());

            await FirebaseFireStore.Instance.AddData(CollectionId, FirebaseAuthentication.Instance.User.UserId, "Data", "Metadata", data);

            data.Clear();
            //data.Add(DATA, new Zitga.Core.Toolkit.Encryption.AesCbcAlgorithm(secret, salt).Encrypt(new Zitga.Core.Toolkit.Compression.GZipAlgorithm().Compress(savePackage.data)));
            data.Add(DATA, savePackage.data);

            await FirebaseFireStore.Instance.AddData(CollectionId, FirebaseAuthentication.Instance.User.UserId, "Data", "Data", data);


        }

        public async UniTask<ESaveStatus> Save()
        {
            Logger.Log("SAVE ");
            UpdateLocal();

            //check cloud

            cloudPackage = await FetchMetadata();
            //if cloud save is not from this device 
            //show compare saves
          
            if (!string.IsNullOrEmpty(cloudPackage.id) &&!cloudPackage.id.Equals(savePackage.id))
            {
                
                return ESaveStatus.Conflict;
            }

            try
            {
                await Push(savePackage);
            }
            catch(System.Exception e)
            {
                Logger.LogError(e);
                return ESaveStatus.Failed;
            }
            Logger.Log("SAVED");
            return ESaveStatus.Success;
        }
       
        //fetch metadata from cloud
        public async UniTask<SavePackage> FetchMetadata()
        {
            //return;
            var result = await FirebaseFireStore.Instance.GetData(CollectionId, FirebaseAuthentication.Instance.User.UserId, "Data", "Metadata");
            if (result.Count != 0)
            {
                cloudPackage.id = result[ID];
                cloudPackage.timeStamp = result[TIME];
                //cloudPackage.data = result[DATA];
                cloudPackage.coin = long.Parse(result[GOLD]);
                cloudPackage.gem = long.Parse(result[GEM]);
                cloudPackage.level = int.Parse(result[LEVEL]);
                cloudPackage.xp = long.Parse(result[XP]);


                Logger.Log(cloudPackage);
            }
            return cloudPackage;
        }

        //fetch game save data from cloud
        public async UniTask<SavePackage> FetchData()
        {
            //return;
            var result = await FirebaseFireStore.Instance.GetData(CollectionId, FirebaseAuthentication.Instance.User.UserId, "Data", "Data");
            if (result.Count != 0)
            {

                cloudPackage.data = result[DATA];



                Logger.Log(cloudPackage);
            }
            return cloudPackage;
        }

        private UserDataCloud NewUserDataCloud()
        {
            return new UserDataCloud
            {
                General = DatasaveManager.Instance.GetFileRawData("General"),
                User = DatasaveManager.Instance.GetFileRawData("User"),
                Inventory = DatasaveManager.Instance.GetFileRawData("Inventory"),
                Resources = DatasaveManager.Instance.GetFileRawData("Resources"),
                Dungeon = DatasaveManager.Instance.GetFileRawData("Dungeon"),
                TryHero = DatasaveManager.Instance.GetFileRawData("TryHero"),
                DungeonSession = DatasaveManager.Instance.GetFileRawData("DungeonSession"),
                Chest = DatasaveManager.Instance.GetFileRawData("Chest"),
                Offer = DatasaveManager.Instance.GetFileRawData("Offer"),
                DungeonEvent = DatasaveManager.Instance.GetFileRawData("DungeonEvent"),
                DungeonEventSession = DatasaveManager.Instance.GetFileRawData("DungeonEventSession"),
                DailyQuest = DatasaveManager.Instance.GetFileRawData("DailyQuest"),
                Achievement = DatasaveManager.Instance.GetFileRawData("Achievement"),
                SkillTree = DatasaveManager.Instance.GetFileRawData("SkillTree"),
                HotSaleHero = DatasaveManager.Instance.GetFileRawData("HotSaleHero"),
                Subscription = DatasaveManager.Instance.GetFileRawData("Subscription"),
                DungeonWorld = DatasaveManager.Instance.GetFileRawData("DungeonWorld"),
                ButtonFeature = DatasaveManager.Instance.GetFileRawData("ButtonFeature"),
                PiggyBank = DatasaveManager.Instance.GetFileRawData("PiggyBanks"),
                FlashSale = DatasaveManager.Instance.GetFileRawData("FlashSale"),
                PlayGift = DatasaveManager.Instance.GetFileRawData("PlayGift"),
            };
        }
        public void OverwriteCurrentUserData(UserDataCloud userData)
        {
            var dataSave = DatasaveManager.Instance;
            if (!string.IsNullOrEmpty(userData.General))
            {
                dataSave.General = dataSave.LoadFromRawData<GeneralSave>(userData.General);
            }
            if (!string.IsNullOrEmpty(userData.User))
            {
                dataSave.User = dataSave.LoadFromRawData<UserSave>(userData.User);
            }

            if (!string.IsNullOrEmpty(userData.Dungeon))
            {
                dataSave.Dungeon = dataSave.LoadFromRawData<DungeonSave>(userData.Dungeon);
            }

            if (!string.IsNullOrEmpty(userData.Achievement))
            {
                dataSave.Achievement = dataSave.LoadFromRawData<AchievementQuestSaves>(userData.Achievement);
            }

            if (!string.IsNullOrEmpty(userData.DailyQuest))
            {
                dataSave.DailyQuest = dataSave.LoadFromRawData<DailyQuestSaves>(userData.DailyQuest);
            }

            if (!string.IsNullOrEmpty(userData.Subscription))
            {
                dataSave.Subscription = dataSave.LoadFromRawData<SubscriptionSaves>(userData.Subscription);
            }

            if (!string.IsNullOrEmpty(userData.Inventory))
            {
                dataSave.Inventory = dataSave.LoadFromRawData<InventorySave>(userData.Inventory);
            }

            if (!string.IsNullOrEmpty(userData.TryHero))
            {
                dataSave.TryHero = dataSave.LoadFromRawData<TryHeroSaves>(userData.TryHero);
            }

            if (!string.IsNullOrEmpty(userData.HotSaleHero))
            {
                dataSave.HotSaleHero = dataSave.LoadFromRawData<HotSaleHeroSaves>(userData.HotSaleHero);
            }

            if (!string.IsNullOrEmpty(userData.Chest))
            {
                dataSave.Chest = dataSave.LoadFromRawData<ChestsSave>(userData.Chest);
            }

            if (!string.IsNullOrEmpty(userData.Offer))
            {
                dataSave.Offer = dataSave.LoadFromRawData<OffersSave>(userData.Offer);
            }

            if (!string.IsNullOrEmpty(userData.ButtonFeature))
            {
                dataSave.ButtonFeature = dataSave.LoadFromRawData<ButtonFeatureSave>(userData.ButtonFeature);
            }

            if (!string.IsNullOrEmpty(userData.DungeonEvent))
            {
                dataSave.DungeonEvent = dataSave.LoadFromRawData<DungeonEventSaves>(userData.DungeonEvent);
            }

            if (!string.IsNullOrEmpty(userData.DungeonEventSession))
            {
                dataSave.DungeonEventSession = dataSave.LoadFromRawData<DungeonEventSessionSave>(userData.DungeonEventSession);
            }

            if (!string.IsNullOrEmpty(userData.DungeonSession))
            {
                dataSave.DungeonSession = dataSave.LoadFromRawData<DungeonSessionSave>(userData.DungeonSession);
            }

            if (!string.IsNullOrEmpty(userData.DungeonWorld))
            {
                dataSave.DungeonWorld = dataSave.LoadFromRawData<DungeonWorldSaves>(userData.DungeonWorld);
            }

            if (!string.IsNullOrEmpty(userData.SkillTree))
            {
                dataSave.SkillTree = dataSave.LoadFromRawData<SkillTreeSaves>(userData.SkillTree);
            }

            if (!string.IsNullOrEmpty(userData.Resources))
            {
                dataSave.Resources = dataSave.LoadFromRawData<ResourcesSave>(userData.Resources);
            }
            
            if (!string.IsNullOrEmpty(userData.PiggyBank))
            {
                dataSave.PiggyBank = dataSave.LoadFromRawData<PiggyBanksSave>(userData.PiggyBank);
            }
            
            if (!string.IsNullOrEmpty(userData.FlashSale))
            {
                dataSave.FlashSale = dataSave.LoadFromRawData<FlashSaleSaves>(userData.FlashSale);
            }
            
            if (!string.IsNullOrEmpty(userData.PlayGift))
            {
                dataSave.PlayGift = dataSave.LoadFromRawData<PlayGiftSaves>(userData.PlayGift);
            }
            dataSave.Add();
            dataSave.FixData();

            dataSave.SaveData();

            ReloadSceneAndApplyNewData();
        }
   
        async UniTask ReloadSceneAndApplyNewData()
        {

            LoadingScreen loadingScreen = (await Addressables.InstantiateAsync("LoadingScreen")).GetComponent<LoadingScreen>();

            loadingScreen.Show();
            SceneLoader sceneLoader = new SceneLoader(SceneKey.MENU);

            //loading progress
            sceneLoader.onSceneLoading += async (loader, progress) =>
            {
                loadingScreen.SetProgress(progress);
            };
            //new scene has been shown
            sceneLoader.onScenePresented += async (loader) =>
            {
                loadingScreen.Hide();
            };
            //on scene loaded
            sceneLoader.onSceneLoaded += async (loader) =>
            {
                await loader.ActiveScene();
            };
            //last scene has been hidden, but not removed
            sceneLoader.onLastSceneHidden += async (loader) => { };
            DataManager.Instance.OnInit();
            GameSceneManager.Instance.InitializePlayerData();
        }
        
    }
}