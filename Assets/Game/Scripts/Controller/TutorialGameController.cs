using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Handler;
using Game.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TutorialGameController : GameController
{
    public int Stage = 0;
    public Character player;
    public LevelBuilderBase levelBuilder;
    public EnemySpawnHandler spawnHandler;
    Portal portal;
    public DungeonEntity dungeonSpawnCurrent;


    bool playThemeNext = true;
    IAssetLoader assetLoader = new AddressableAssetLoader();

    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player startFb;

    [SerializeField]

    private GameTutSO gameTut;
    public override void Destroy()
    {
        Debug.Log("DESTROY");
        Game.Pool.GameObjectSpawner.Instance.Destroy(true);
        Sound.Controller.Instance.StopMusic();
        RemoveCalculateDPSListener(player?.Stats);
        levelBuilder.Destroy();
        spawnHandler.Destroy();
        Messenger.RemoveListener<bool>(EventKey.StageFinish, OnStageFinish);
        Messenger.RemoveListener(EventKey.TriggerPortal, OnPortalTriggered);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
        assetLoader.ReleaseAll();
        gameTut.Clear();
        base.Destroy();
    }

    //init gamecontroller, prepare everything
    public override async UniTask Initialize()
    {
        dungeonSpawnCurrent = DataManager.Base.Dungeon.GetDungeon(-1);

        //prepare player
        player = await PreparePlayer();
        player.SetActive(false);

        LoadMap();

        AimCrossHair crossHair = (await Game.Pool.GameObjectSpawner.Instance.GetAsync("AimCrossHair", Game.Pool.EPool.Pernament)).GetComponent<AimCrossHair>();
        crossHair.SetUp(player);
        Messenger.Broadcast(EventKey.ActorSpawn, (ActorBase)player, false, -1);

        //prepare event callback
        Messenger.AddListener<bool>(EventKey.StageFinish, OnStageFinish);
        Messenger.AddListener(EventKey.TriggerPortal, OnPortalTriggered);
        Messenger.AddListener<Callback>(EventKey.StageStart, OnGameStart);


        //
        await PrepareLevel();
        startFb.PlayFeedbacks();

        StartBattle().Forget();


        gameTut.SetUp(this);


        Sound.Controller.Instance.soundData.PlayGameTheme();


    }

    private void LoadMap()
    {
        var listRoomId = new List<int>();

        // get dungeon data
        var dungeon = DataManager.Base.Dungeon.GetDungeon(-1);
        dungeonSpawnCurrent = new DungeonEntity(-1);
        foreach (var stages in dungeon.Stages)
        {
            var newStage = new DungeonStageEntity();
            var allRoomId = new List<int>();
            foreach (var rm in stages.Item1.RoomRandom)
            {
                allRoomId.Add(rm.RoomId);
            }

            var roomId = allRoomId.Random(listRoomId);
            listRoomId.Add(roomId);

            var room = DataManager.Base.Room.GetRoom(roomId);

            room.StageLinked = stages.Item1;
            newStage.RoomRandom.Add(room);
            newStage.RewardStage = stages.Item1.RewardStage;


            newStage.LevelEnemyStage = stages.Item1.LevelEnemyStage;
            dungeonSpawnCurrent.AddStage(newStage, stages.Item2);
        }
    }

    #region GAME EVENTS

    //when finish stage start animation,call by startfeedback
    private void OnGameStart(Callback cb)
    {
        GameUIPanel.Instance.SetUp(this);
        GameUIPanel.Instance.pauseBtn.SetActive(false);

        if(HealthBarHandler.Instance.Get(GetMainActor())!=null)
            HealthBarHandler.Instance.Get(GetMainActor()).SetActive(true);
        GetMainActor().StartBehaviours();
        isReady = true;
        onStageStart?.Invoke();
     
        GetMainActor().PropertyHandler.AddProperty(EActorProperty.Vunerable, 0);

    }
    //on any actor die
    protected override void OnDie(ActorBase actor, ActorBase damageSource)
    {
    }
    
    //when go through the PORTAL
    void OnStageFinish(bool buffPortal)
    {
        //load next stage
        Finish().Forget();
    }

   
    //trigger by portal
    public void OnPortalTriggered()
    {
        CameraController.Instance.Follow(portal.transform, new Vector2(0, 0), false);
        //CameraController.Instance.SetBoundary(null);
        if(HealthBarHandler.Instance.Get(GetMainActor())!=null)
            HealthBarHandler.Instance.Get(GetMainActor()).SetActive(false);

        GetMainActor().BehaviourHandler.StopBehaviours();
        GameUIPanel.Instance.Hide();
        Messenger.AddListener(EventKey.PlayerTeleported, OnPlayerTeleported);

    }
    //trigger by portal
    void OnPlayerTeleported()
    {
        GetMainActor().SetActive(false);
        Messenger.RemoveListener(EventKey.PlayerTeleported, OnPlayerTeleported);

    }
    #endregion

    //preprare map, enemy spawn
    public async override UniTask PrepareLevel(int room = -1)
    {
        CameraController.Instance.GetTransform().localPosition = Vector3.zero;

        Game.Pool.GameObjectSpawner.Instance.Destroy(false);
        spawnHandler.ClearAll();
        isFinished = false;
        levelBuilder.Destroy();
        Map map = null;
        BackGround backGround = null;

        DungeonRoomEntity roomConfig = dungeonSpawnCurrent.GetStage(Stage).GetRoom();
        //creating  platform and background, portal
        map = await levelBuilder.SetUp(string.Format(AddressableName.WorldMap, roomConfig.Map));
        backGround = await levelBuilder.SetUpBackGround(gameModeData.GetBackGround(0, map.roomType).RuntimeKey.ToString());
        portal = await levelBuilder.SetUpPortal(false);
        spawnHandler.SetUp(map.groupNpcSpawn, roomConfig);



        CameraController.Instance.SetBoundary(map.boundary);
        //prepare player
        GetMainActor().SetPosition(map.playerSpawnPoint.position);
        GetMainActor().SetActive(true);


        onStageReady?.Invoke((int)gameModeData.gameMode, 0,0,EDungeonEvent.None);
    }


    //game start
    public override async UniTask StartBattle()
    {
        CameraController.Instance.Follow(GetMainActor().GetMidTransform(), Vector2.zero, true);
        await UniTask.Delay(1000, ignoreTimeScale: true, cancellationToken: cancellationToken.Token);


    }

    async UniTask<PlayerController> PreparePlayer()
    {
        var mainWeapon = GameSceneManager.Instance.PlayerData.EquipmentHandler.GetEquipment(EEquipment.MainWeapon);
        Debug.Log(mainWeapon.Id);
        //Equipab mainWeapon = null;
        PlayerController player = (await Game.Pool.GameObjectSpawner.Instance.GetAsync("Player", Game.Pool.EPool.Pernament)).GetComponent<PlayerController>();

        WeaponBase playerWeapon = await assetLoader.LoadAsync<WeaponBase>("Weapon_" + (mainWeapon == null ? "4" : mainWeapon.Id)).Task;
        var weaponData = new WeaponData
        {
            Weapon = playerWeapon,
            Item = mainWeapon
        };
        player.WeaponHandler.LoadWeapon(playerWeapon, weaponData);
        var stats = GetStatPlayer();

        // Add DPS change listener

        await player.SetUp(stats);
        CalculateDPS(stats);

        player.PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);
        player.AnimationHandler.SetSkin("-1");
        return player;

    }

    #region Add DPS Event
    private void CalculateDPS(IStatGroup stats)
    {
    }
    private void RemoveCalculateDPSListener(IStatGroup stats)
    {
    }
    private void OnRecalculateNewDPS(float value)
    {
    }
    #endregion

    #region get
    //get
    private IStatGroup GetStatPlayer()
    {
        var stats = GameSceneManager.Instance.PlayerData.Stats;
        stats.CalculateStats();
        return stats;
    }

    public override LevelBuilderBase GetLevelBuilder()
    {
        return levelBuilder;
    }

    public override EnemySpawnHandler GetEnemySpawnHandler()
    {
        return spawnHandler;
    }

    public override ActorBase GetMainActor()
    {
        return player;
    }
    #endregion

    public override async UniTask Finish()
    {
        if (isFinished) return;
        Sound.Controller.Instance.soundData.PlayWinSFX();
        await base.Finish();

        //
        //to next stage;
        Stage++;
        if (Stage >= dungeonSpawnCurrent.Stages.Count)
        {
            DataManager.Save.General.IsGameTutFinished = true;
            await UniTask.Delay(500);
            Game.Transitioner.Controller.Instance.Trigger();
            await UniTask.Delay(500);
            Game.Transitioner.Controller.Instance.Release();
            DataManager.Save.DungeonSession.Clear();
            DataManager.Save.SaveData();

            GameUIPanel.Instance.pauseBtn.SetActive(true);
            Game.Controller.Instance.StartLevel(GameMode.Normal,0);

        }
        else
        {
            await Game.Transitioner.Controller.Instance.Trigger(gameModeData.GetWorldColor(0));
            await UniTask.Delay(250);
            await PrepareLevel();
            Game.Transitioner.Controller.Instance.Release();

            CameraController.Instance.ResetCameraSize();
            startFb.PlayFeedbacks();

            await StartBattle();
        }

    }

  
}
