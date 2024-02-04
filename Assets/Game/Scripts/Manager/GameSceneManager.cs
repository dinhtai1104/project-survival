using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent;
using com.debug.log;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameSceneManager : LiveSingleton<GameSceneManager>
{
    [SerializeField] private AssetReferenceT<TMPro.TMP_FontAsset> BaseFontRef;
    [SerializeField] private TMPro.TMP_FontAsset _baseFontAsset;
    [SerializeField]
    private EquipmentFactory equipmentFactory;
    public EquipmentFactory EquipmentFactory => equipmentFactory;

    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData => playerData;
    public DiscordAPI Discord;

    private void Start()
    {
        DOTween.defaultTimeScaleIndependent = true;
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        Addressables.LoadAssetAsync<TMP_FontAsset>(BaseFontRef).Completed += GameSceneManager_Completed;

        GameArchitecture.Instance.Inject();
        InitializePlayerData();
        IAPManager.Instance.Init(DataManager.Base.IapConfig.Dictionary.Values.ToList());

        GameArchitecture.Instance.StartServices();

#if DEVELOPMENT && !UNITY_EDITOR
        Discord = new DiscordAPI();
        Application.logMessageReceived += LogCallback;
#endif
    }

    private void GameSceneManager_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<TMP_FontAsset> obj)
    {
        _baseFontAsset = obj.Result;
        UpdateTmpFontAsset(I2.Loc.LocalizationManager.CurrentLanguage).Forget();
    }

    private void LogCallback(string log, string track, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {   
            var logMessage = $"{log}\nStackTrace: {track}";
            StartCoroutine(Discord.PostMessage(logMessage));
        }
    }
    public void InitializePlayerData()
    {
        var userSave = DataManager.Save.User;
        var playerStat = PlayerStat.Default();
        playerData.HeroCurrent = userSave.HeroCurrent;
        var entity = DataManager.Base.Hero.GetHero(playerData.HeroCurrent);
        entity.SetBaseStat(playerStat, EStatSource.Hero);
        
        var equipmentHandler = new EquipmentHandler(playerStat);
        var expHandler = new ExpHandler(userSave.Experience, DataManager.Base.ExpRequire.Dictionary.Values.ToList());

        playerData = new PlayerData
        (
            playerStat,
            userSave.Hero,
            equipmentHandler,
            expHandler
        );

        playerData.Init(userSave.HeroCurrent);

        equipmentFactory = new EquipmentFactory(playerData);

        DataManager.Save.Inventory.CreateAllEquipment(equipmentFactory);
        var eqHandler = playerData.EquipmentHandler;
        equipmentFactory.EquipItem(eqHandler);
    }

    public async UniTask UpdateTmpFontAsset(string language)
    {
        for (var i = _baseFontAsset.fallbackFontAssetTable.Count - 1; i >= 1; i--)
        {
            if (_baseFontAsset.fallbackFontAssetTable[i] == null)
            {
                _baseFontAsset.fallbackFontAssetTable.RemoveAt(i);
                continue;
            }
            var name = _baseFontAsset.fallbackFontAssetTable[i].name;
            ResourcesLoader.Instance.UnloadAsset<TMP_FontAsset>($"{"FallbackTMP"}/{name}.asset");
            _baseFontAsset.fallbackFontAssetTable.RemoveAt(i);
        }

        if (language.Equals(_baseFontAsset.name)) return;

        var fallbackFontAsset = await ResourcesLoader.Instance.LoadAsync<TMP_FontAsset>($"{"FallbackTMP"}/{language}.asset");
        _baseFontAsset.fallbackFontAssetTable.Add(fallbackFontAsset);

        Messenger.Broadcast(EventKey.ChangeLanguage);

        var all = FindObjectsOfType<I2.Loc.Localize>();
        foreach (var i2 in all)
        {
            i2.gameObject.SetActive(false);
            i2.gameObject.SetActive(true);
        }
    }

    public void PrintLog(string print)
    {
#if DEVELOPMENT
        if (Discord == null)
        {
            Discord = new DiscordAPI();
        }
        StartCoroutine(Discord.PostMessage(print));
#endif
    }
    public void PrintLogInProduction(string print)
    {
        if (Discord == null)
        {
            Discord = new DiscordAPI();
        }
        StartCoroutine(Discord.PostMessage(print));
    }
}