using UnityEngine;
using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using Assets.Game.Scripts.DungeonWorld.Save;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System.IO;
using System.Text;
using BayatGames.SaveGameFree.Encoders;
using Sirenix.Serialization;
using DG.Tweening.Plugins.Core.PathCore;
using FullSerializer;
using Newtonsoft.Json;
using NSubstitute.Routing.Handlers;
using Assets.Game.Scripts.Talent.Datasave;

public class DatasaveManager : MonoSingleton<DatasaveManager>
{
    private bool encode = true;
    private string password = "dungeon_labs";

    private List<IDatasave> datasaves = new List<IDatasave>();

    public GeneralSave General;
    public UserSave User;
    public InventorySave Inventory;
    public ResourcesSave Resources;
    public DungeonSave Dungeon;
    public TryHeroSaves TryHero;

    public DungeonSessionSave DungeonSession;
    public ChestsSave Chest;
    public OffersSave Offer;

    public DungeonEventSaves DungeonEvent;
    public DungeonEventSessionSave DungeonEventSession;

    public DailyQuestSaves DailyQuest;
    public AchievementQuestSaves Achievement;

    public SkillTreeSaves SkillTree;
    public HotSaleHeroSaves HotSaleHero;
    public SubscriptionSaves Subscription;
    public DungeonWorldSaves DungeonWorld;
    public ButtonFeatureSave ButtonFeature;
    public PiggyBanksSave PiggyBank;
    public FlashSaleSaves FlashSale;
    public PlayGiftSaves PlayGift;

    public BattlePassSaves BattlePass;
    public TalentsSave Talent;

    public void Init(Transform parent = null)
    {
        DataManager.Save = this;
        if (parent) transform.SetParent(parent);

#if UNITY_EDITOR
        encode = false;
#endif
        SaveGame.Encode = encode;
        SaveGame.EncodePassword = password;
        SaveGame.Serializer = new SaveGameJsonSerializer();
        LoadData();
    }
    public void LoadData()
    {
        User = SaveGame.Load("User", new UserSave("User"));

        //General = Load<GeneralSave>("General");
        General = SaveGame.Load("General", new GeneralSave("General"));

        Inventory = SaveGame.Load("Inventory", new InventorySave("Inventory"));
        
        //Subscription = Load<SubscriptionSaves>("Subscription");
        Subscription = SaveGame.Load("Subscription", new SubscriptionSaves("Subscription"));

        //Resources = Load<ResourcesSave>("Resources");
        Resources = SaveGame.Load("Resources", new ResourcesSave("Resources"));//Resources

        //Dungeon = Load<DungeonSave>("ZoneSave");
        Dungeon = SaveGame.Load("Dungeon", new DungeonSave("Dungeon"));

        //TryHero = Load<TryHeroSaves>("TryHero");
        TryHero = SaveGame.Load("TryHero", new TryHeroSaves("TryHero"));

        //DungeonSession = Load<DungeonSessionSave>("DungeonPlaying");
        DungeonSession = SaveGame.Load("DungeonSession", new DungeonSessionSave("DungeonSession"));

        //Chest = Load<ChestsSave>("Chest");
        Chest = SaveGame.Load("Chest", new ChestsSave("Chest"));

        //Offer = Load<OffersSave>("Offer");
        Offer = SaveGame.Load("Offer", new OffersSave("Offer"));

        //DungeonEvent = Load<DungeonEventSaves>("DungeonEvent");
        DungeonEvent = SaveGame.Load("DungeonEvent", new DungeonEventSaves("DungeonEvent"));

        //DungeonEventSession = Load<DungeonEventSessionSave>("DungeonEventSession");
        DungeonEventSession = SaveGame.Load("DungeonEventSession", new DungeonEventSessionSave("DungeonEventSession"));

       // DailyQuest = Load<DailyQuestSaves>("DailyQuest");
        DailyQuest = SaveGame.Load("DailyQuest", new DailyQuestSaves("DailyQuest"));
        
        //Achievement = Load<AchievementQuestSaves>("Achievement");
        Achievement = SaveGame.Load("Achievement", new AchievementQuestSaves("Achievement"));

        //SkillTree = Load<SkillTreeSaves>("SkillTree");
        SkillTree = SaveGame.Load("SkillTree", new SkillTreeSaves("SkillTree"));

        //HotSaleHero = Load<HotSaleHeroSaves>("HotSaleHero");
        HotSaleHero = SaveGame.Load("HotSaleHero", new HotSaleHeroSaves("HotSaleHero"));

        //DungeonWorld = Load<DungeonWorldSaves>("DungeonWorld");
        DungeonWorld = SaveGame.Load("DungeonWorld", new DungeonWorldSaves("DungeonWorld"));
        
        ButtonFeature = SaveGame.Load("ButtonFeature", new ButtonFeatureSave("ButtonFeature"));

        PiggyBank = SaveGame.Load("PiggyBanks", new PiggyBanksSave("PiggyBanks"));

        FlashSale = SaveGame.Load("FlashSale", new FlashSaleSaves("FlashSale"));

        PlayGift = SaveGame.Load("PlayGift", new PlayGiftSaves("PlayGift"));

        BattlePass = SaveGame.Load("BattlePass", new BattlePassSaves("BattlePass"));

        Talent = SaveGame.Load("Talent", new TalentsSave("Talent"));

        Add();
    }
    private void Update()
    {
        General.TimeOnline += Time.deltaTime;
    }
    public void Add()
    {
        datasaves.Clear();
        datasaves.Add(User);
        datasaves.Add(General);
        datasaves.Add(Inventory);
        datasaves.Add(Subscription);
        datasaves.Add(Resources);
        datasaves.Add(Dungeon);
        datasaves.Add(TryHero);
        datasaves.Add(DungeonSession);
        datasaves.Add(Chest);
        datasaves.Add(Offer);
        datasaves.Add(DungeonEvent);
        datasaves.Add(DungeonEventSession);
        datasaves.Add(DailyQuest);
        datasaves.Add(Achievement);
        datasaves.Add(SkillTree);
        datasaves.Add(HotSaleHero);
        datasaves.Add(DungeonWorld);
        datasaves.Add(ButtonFeature);
        datasaves.Add(PiggyBank);
        datasaves.Add(FlashSale);
        datasaves.Add(PlayGift);
        datasaves.Add(BattlePass);
        datasaves.Add(Talent);
    }
    public TData Load<TData>(string key) where TData : BaseDatasave
    {
        TData data = SaveGame.Load<TData>(key, (TData)Activator.CreateInstance(typeof(TData), key));
        datasaves.Add(data);

        return data;
    }

    public void FixData()
    {
        foreach (var save in datasaves)
        {
            save.Fix();
        }

        if (UnbiasedTime.UtcNow.DayOfYear != General.LastTimeOut.DayOfYear)
            NextDay();
    }

    public void SaveData()
    {
        datasaves.Clear();
        Add();
        foreach (var save in datasaves)
        {
            save.Save();
        }

    }
    public List<IDatasave> GetSave()
    {
        return datasaves;
    }
    public void NextDay()
    {
        Debug.Log("Next Day");
        foreach (var save in datasaves)
        {
            save.NextDay();
            save.Save();
        }
        General.SetLastTimeOut();
    }

    public void OnLoaded()
    {
        foreach (var save in datasaves)
        {
            save.OnLoaded();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            SaveData();
        }
    }

    [Button]
    public string GetDecodeData(string datajson)
    {
        var obj = JsonConvert.DeserializeObject<GeneralSave>(datajson);
        ISaveGameSerializer serializer = null;
        var encoder = new SaveGameSimpleEncoder();
        if (serializer == null)
        {
            serializer = SaveGame.Serializer;
        }
        var encoding = SaveGame.DefaultEncoding;
        if (encoding == null)
        {
            encoding = SaveGame.DefaultEncoding;
        }
       
        Stream stream = null;
        stream = new MemoryStream();
        
        serializer.Serialize(obj, stream, encoding);
        string data = System.Convert.ToBase64String(((MemoryStream)stream).ToArray());
        string encoded = encoder.Encode(data, password);
        
        stream.Dispose();

        return encoded;
    }

    [Button]
    public string GetDataEnCode(string data)
    {
        var result = "";
        var decoded = SaveGame.Encoder.Decode(data, password);
        var stream = new System.IO.MemoryStream(Convert.FromBase64String(decoded), true);
        using (var reader = new System.IO.StreamReader(stream, SaveGame.DefaultEncoding))
        {
            result = reader.ReadToEnd();
        }

        stream.Dispose();
        return result;
    }

    #region FIND FILE TO CUSTOM SAVE
    [Button]
    public string GetFileRawData(string fileName)
    {
        try
        {
            if (SaveGame.Exists(fileName))
            {
                var filePath = $"{Application.persistentDataPath}/Save/{fileName}";
                var data = System.IO.File.ReadAllText(filePath, SaveGame.DefaultEncoding);
                if (encode)
                {
                    var result = "";
                    var decoded = SaveGame.Encoder.Decode(data, password);
                    var stream = new System.IO.MemoryStream(Convert.FromBase64String(decoded), true);
                    using (var reader = new System.IO.StreamReader(stream, SaveGame.DefaultEncoding))
                    {
                        result = reader.ReadToEnd();
                    }

                    stream.Dispose();
                    return result;
                }

                return data;
            }

            return string.Empty;
        }
        catch (Exception e)
        {
            Debug.LogError($"Get file raw data Failed {e.Message} {e.StackTrace}\n{fileName}");
            return string.Empty;
        }
    }
    public T LoadFromRawData<T>(string rawData)
    {
        try
        {
            var stream = new System.IO.MemoryStream(SaveGame.DefaultEncoding.GetBytes(rawData));
            var saveObj = SaveGame.Serializer.Deserialize<T>(stream, SaveGame.DefaultEncoding);
            stream.Dispose();
            return saveObj;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load raw data Failed {e.Message} {e.StackTrace}\n{rawData}");
            return default(T);
        }
    }

    public void ClearSession()
    {
        DungeonSession?.Clear();
        DungeonEventSession?.Clear();
    }

    public void ClearData()
    {
        datasaves.Clear();
        SaveGame.Clear();
        LoadData();
    }

    #endregion
}