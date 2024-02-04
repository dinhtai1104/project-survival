using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Effect;
using Foundation.Game.Time;
using Game.GameActor;
using Game.Handler;
using Game.Level;
using Game.Skill;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public enum ERoomType
{
    Normal,Angel,Supply,Boss
}
public class BattleGameController : GameController
{
    //id
    public int Dungeon, Stage, Wave;
    public ERoomType currentRoomType;
    public Character player,drone;
    public LevelBuilderBase levelBuilder;
    public EnemySpawnHandler spawnHandler;

    AimCrossHair crossHair;
    Portal portal;
    bool playThemeNext=true;
    //
    public DungeonEntity dungeonSpawnCurrent;
    IAssetLoader assetLoader=new AddressableAssetLoader();

    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player startFb;

    // Session
    protected DungeonSessionSave currentSessionSave;
    protected DungeonSave dungeonSave;
    protected BattleSession battleSession;

    // Buff Count
    /// <summary>
    /// Count of Buff if it > 0, pick until end
    /// </summary>
    protected int BuffPickCount = 0;

    public override void Destroy()
    {
        Game.Pool.GameObjectSpawner.Instance.Destroy(true);
        Sound.Controller.Instance.StopMusic();
        RemoveCalculateDPSListener(player?.Stats);
        levelBuilder.Destroy();
        spawnHandler.Destroy();
        Messenger.RemoveListener<bool>(EventKey.StageFinish, OnStageFinish);
        Messenger.RemoveListener(EventKey.SelectBuff, OnShowBuffSelection);
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<EPickBuffType>(EventKey.PickBuffDone, OnBuffSelected);
        Messenger.RemoveListener(EventKey.TriggerPortal, OnPortalTriggered);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
        Messenger.RemoveListener<Drone>(EventKey.DroneSpawn, OnDroneSpawn);
        Messenger.RemoveListener<ActorBase, int, int>(EventKey.OnCastSkill, OnCastSkill);
        Messenger.RemoveListener<EBuff>(EventKey.CastBuffToMainPlayer, OnCastBuff);
        Messenger.RemoveListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);
        assetLoader.ReleaseAll();
        //player.onRevive -= OnMainPlayerReviveResult;

        base.Destroy();
    }
  
    public override async UniTask SetPlaySession(DungeonSessionSave save)
    {
        currentSessionSave = save;
    }

    public virtual void LoadMap()
    {
        Dungeon = currentSessionSave.CurrentDungeon;
        Stage = currentSessionSave.CurrentStage;
        dungeonSave = DataManager.Save.Dungeon;
        currentSessionSave.JoinDungeon(Dungeon);

        // if not cached before last play => create new cached map
        if (currentSessionSave.MemoryMap == null || currentSessionSave.MemoryMap.roomId.Count == 0)
        {
            // cached memory map
            var mapMemory = new DungeonSessionMemoryMap();
            var listRoomId = new List<int>();

            // get dungeon data
            var dungeon = DataManager.Base.Dungeon.GetDungeon(Dungeon);
            dungeonSpawnCurrent = new DungeonEntity(Dungeon);
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
            mapMemory.SetIdRooms(listRoomId);
            currentSessionSave.SetMemoryMap(mapMemory);
        }
        else
        {
            // Load map from cached
            var dungeon = DataManager.Base.Dungeon.GetDungeon(Dungeon);
            dungeonSpawnCurrent = new DungeonEntity(Dungeon);
            int stageCurrent = 0;
            foreach (var roomId in currentSessionSave.MemoryMap.roomId)
            {
                var stages = dungeon.GetStage(stageCurrent);
                var newStage = new DungeonStageEntity();
                var room = DataManager.Base.Room.GetRoom(roomId);
                room.StageLinked = stages;
                newStage.RoomRandom.Add(room);
                newStage.RewardStage = stages.RewardStage;
                newStage.LevelEnemyStage = stages.LevelEnemyStage;

                dungeonSpawnCurrent.AddStage(newStage, dungeon.GetBuffStage(stageCurrent++));
            }
        }
    }
    //init gamecontroller, prepare everything
    public override async UniTask Initialize()
    {
        LoadMap();


        //prepare player
        player = await PreparePlayer();
        player.SetActive(false);

        //player.onRevive += OnMainPlayerReviveResult;
       

        crossHair = (await Game.Pool.GameObjectSpawner.Instance.GetAsync("AimCrossHair",Game.Pool.EPool.Pernament)).GetComponent<AimCrossHair>();
        crossHair.SetUp(player);
        Messenger.Broadcast(EventKey.ActorSpawn, (ActorBase)player, false,-1);

        //prepare event callback
        Messenger.AddListener<bool>(EventKey.StageFinish, OnStageFinish);
        Messenger.AddListener(EventKey.SelectBuff, OnShowBuffSelection);
        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.AddListener(EventKey.TriggerPortal, OnPortalTriggered);
        Messenger.AddListener<EPickBuffType>(EventKey.PickBuffDone, OnBuffSelected);
        Messenger.AddListener<EBuff>(EventKey.CastBuffToMainPlayer, OnCastBuff);
        Messenger.AddListener<Callback>(EventKey.StageStart, OnGameStart);
        Messenger.AddListener<Drone>(EventKey.DroneSpawn, OnDroneSpawn);
        Messenger.AddListener<ActorBase, int, int>(EventKey.OnCastSkill, OnCastSkill);
        Messenger.AddListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);

        //load and init every equipment with passive
        GameSceneManager.Instance.PlayerData.LoadEquipmentPassive(player);

        //load last save if possible
        await LoadLastSave();

        //
        await PrepareLevel();
        startFb.PlayFeedbacks();

        StartBattle().Forget();


        DataManager.Save.General.PlaySession++;

    }

    

    private void OnCastBuff(EBuff buff)
    {
        battleSession.Buff.Add((int)buff);
    }
    public override void RerollBuff(string reroll)
    {
        base.RerollBuff(reroll);
        battleSession.Reroll = reroll;
    }
    async UniTask LoadLastSave()
    {
        await LoadLastBuffSave();
        LoadLastPlayerSave();

        async UniTask LoadLastBuffSave()
        {
            var data = currentSessionSave.buffSession;
            foreach (var buff in data.Dungeon.BuffEquiped)
            {
                var buffSave = buff.Value;
                for (int i = 0; i < buffSave.Level; i++)
                {
                    await GetMainActor().BuffHandler.Cast(buff.Key, false);
                }
            }
        }
        void LoadLastPlayerSave()
        {
            GetMainActor().HealthHandler.SetHealth(currentSessionSave.CurrentHpPercent * player.HealthHandler.GetMaxHP());
        }


    }


    #region GAME EVENTS
    private void OnChangePlayer(ActorBase arg1)
    {
        crossHair.SetUp(arg1);
    }
    private void OnCastSkill(ActorBase arg1, int arg2, int arg3)
    {
        if (arg1 == drone)
        {
            currentSessionSave.DroneTotalCast++;
        }
    }

    //when a drone is spawned
    private void OnDroneSpawn(Drone drone)
    {
        this.drone = drone;
        var skill = drone.SkillEngine.GetSkill(0) as MultiTaskSkill;
        skill.totalCast = currentSessionSave.DroneTotalCast;
    }
    //when finish stage start animation,call by startfeedback
    private void OnGameStart(Callback cb)
    {
        GameUIPanel.Instance.SetUp(this);
        if(HealthBarHandler.Instance.Get(GetMainActor())!=null)
            HealthBarHandler.Instance.Get(GetMainActor()).SetActive(true);
        GetMainActor().StartBehaviours();
        drone?.StartBehaviours();
        isReady = true;
        onStageStart?.Invoke();
        switch (currentRoomType)
        {
            case ERoomType.Angel:
                Sound.Controller.Instance.soundData.PlayFountainTheme();
                break;
            case ERoomType.Normal:
                if (playThemeNext)
                {
                    Sound.Controller.Instance.soundData.PlayGameTheme();
                    playThemeNext = false;
                }

                break;
        }
    }
    //on any actor die
    ActorBase lastActorDie;
    protected override void OnDie(ActorBase actor, ActorBase damageSource)
    {
        if (actor.Tagger.HasTag(ETag.Player))
        {
            if (actor.Stats.GetValue(StatKey.Revive) <= 0)
            {
                //Lose();
            }
        }
        if (actor.GetCharacterType()==ECharacterType.Boss)
        {
            currentSessionSave.TotalBossDefeated++;
            lastActorDie = actor;
        }

       
    }


    //a buff card has been selected
    private void OnBuffSelected(EPickBuffType type)
    {
        BuffPickCount--;
        if (BuffPickCount <= 0)
        {
            switch (type)
            {
                case EPickBuffType.None:
                    break;
                case EPickBuffType.Normal:
                    //load next stage
                    Finish().Forget();
                    break;
                case EPickBuffType.Angel:
                case EPickBuffType.Offer:
                    break;
            }
            return;
        }

        Messenger.Broadcast(EventKey.SelectBuff);

        //
    }

    // when stage clear
    void OnGameClear(bool instantClear)
    {
        isStageCleared = true;
        Game.Pool.GameObjectSpawner.Instance.ClearPool(Game.Pool.EPool.Projectile,true);
        GetMainActor().PropertyHandler.AddProperty(EActorProperty.Vunerable, 0);

        //after boss died
        if (lastActorDie != null)
        {
            //send event chest spawn => barrier will wait for chest finish
            Messenger.Broadcast(EventKey.BossChestSpawned);

            UI.PanelManager.CreateAsync<BossClearPanel>(AddressableName.BossClearPanel).ContinueWith( panel =>
             {
                 panel.SetUp();
                 panel.onClosed = async () =>
                 {
                     Logger.Log("ON CLOSE " + panel);
                     await UniTask.Delay(500);
                     HandleBossSpecialChest(lastActorDie, Stage + 1).Forget();
                     HandleHealthOrb(lastActorDie);
                 };
             }).Forget();

          


            async UniTask HandleBossSpecialChest(ActorBase character, int stage)
            {
                switch (currentSessionSave.TotalBossDefeated)
                {
                    case 1:
                    case 2:
                        Game.Pool.GameObjectSpawner.Instance.Get("SilverBossChest", obj =>
                        {
                            obj.transform.position = character.GetMidTransform().position;
                            obj.gameObject.SetActive(true);
                        });
                        break;
                    case 3:
                        Game.Pool.GameObjectSpawner.Instance.Get("GoldenBossChest", obj =>
                        {
                            obj.transform.position = character.GetMidTransform().position;
                            obj.gameObject.SetActive(true);
                        });
                        break;
#if UNITY_EDITOR
                    default:
                        Game.Pool.GameObjectSpawner.Instance.Get("GoldenBossChest", obj =>
                        {
                            obj.transform.position = character.GetMidTransform().position;
                            obj.gameObject.SetActive(true);
                        });
                        break;
#endif
                }

            }
            async UniTask HandleHealthOrb(ActorBase character)
            {
                float healRate = UnityEngine.Random.Range(new ValueConfigSearch("[Boss_HealthOrb]HealRate_Min").FloatValue, new ValueConfigSearch("[Boss_HealthOrb]HealRate_Max").FloatValue);
                var orb = (await Game.Pool.GameObjectSpawner.Instance.GetAsync("HealthOrb")).GetComponent<HealthOrb>();
                orb.SetUp(healRate, character.GetMidTransform().position);
            }
        }
    }
    //when go through the PORTAL
    protected virtual void OnStageFinish(bool buffPortal)
    {
        if (buffPortal)
        {
            //show buff select screen
            Messenger.Broadcast(EventKey.SelectBuff);
        }
        else
        {
                
            //load next stage
            Finish().Forget();
        }
    }

    void OnShowBuffSelection()
    {
        UI.PanelManager.CreateAsync(AddressableName.UIBuffChoicePanel).ContinueWith(panel =>
        {
            ((UIBuffChoicePanel)panel).Show();
        }).Forget();
    }
    //trigger by portal
    public void OnPortalTriggered()
    {
        CameraController.Instance.Follow(portal.transform,new Vector2(0,0),false);
        //CameraController.Instance.SetBoundary(null);
        //throw new Exception("test");
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
        drone?.SetActive(false);
        Messenger.RemoveListener(EventKey.PlayerTeleported, OnPlayerTeleported);

    }
    #endregion

    //preprare map, enemy spawn
    public async override UniTask PrepareLevel(int room=-1)
    {


        battleSession = new BattleSession
        {
            Dungeon = Dungeon,
            Stage = Stage,
            Mode = currentSessionSave.Mode,
            Buff = new List<int>()
        };

        TimeStartGame = UnbiasedTime.UtcNow;
        Logger.Log($"PREPARE LEVEL: {Dungeon} => {Stage}");

        CameraController.Instance.GetTransform().localPosition = Vector3.zero;

        lastActorDie = null;
        Game.Pool.GameObjectSpawner.Instance.Destroy(false);
        spawnHandler.ClearAll();
        isStageCleared = false;
        isFinished = false;
        levelBuilder.Destroy();
       
        BuffPickCount = dungeonSpawnCurrent.GetBuffStage(Stage);
//

        DungeonRoomEntity dungeonRoom = room == -1 ? dungeonSpawnCurrent.GetStage(Stage).GetRoom() : DataManager.Base.Room.GetRoom(room);
        Logger.Log("=> " + dungeonRoom.Map+" "+dungeonRoom.RoomId);


        //creating  platform and background, portal
        Map map = null;
        map = await levelBuilder.SetUp(string.Format(AddressableName.WorldMap,dungeonRoom.Map));


        await SetUpBackGround(Dungeon, map.roomType);
        portal=await levelBuilder.SetUpPortal(dungeonSpawnCurrent.IsBuffStage(Stage));
        //portal.GetComponent<Portal>().SetUp(Dungeon);
        spawnHandler.SetUp(map.groupNpcSpawn, dungeonRoom);

        currentRoomType = map.roomType;

        

        CameraController.Instance.SetBoundary(map.boundary);
        //prepare player
        GetMainActor().SetPosition(map.playerSpawnPoint.position);
        GetMainActor().SetActive(true);
        //prepare drone
        Drone?.SetActive(true);
        Drone?.SetPosition(GetMainActor().GetMidTransform().position);

        SetStageReady();

        Messenger.Broadcast(EventKey.EnterRoom, map.roomType);
   
        // Save Stage Session
        currentSessionSave.SetHp(GetMainActor().HealthHandler.GetPercentHealth());
        currentSessionSave.JoinStage(Stage);

        if (dungeonSave.CurrentDungeon == Dungeon)
        {
            if (dungeonSave.BestStage < Stage)
            {
                dungeonSave.BestStage = Stage;
                dungeonSave.Save();
            }
        }
        var dataSave = DataManager.Save.TryHero;
#if !UNITY_EDITOR
        heroTrial=true;
#endif
        if (currentSessionSave.Mode == GameMode.Normal && heroTrial)
        {
            HandleHeroTrial();
        }
    }
    public bool heroTrial=true;
    protected virtual void SetStageReady()
    {
        onStageReady?.Invoke((int)gameModeData.gameMode, Dungeon, Stage,EDungeonEvent.None);

    }
    protected virtual async UniTask SetUpBackGround(int dungeon,ERoomType roomType)
    {
        BackGround backGround = null;
        string backGroundId = gameModeData.GetBackGround(dungeon, roomType).RuntimeKey.ToString();
        backGround = await levelBuilder.SetUpBackGround(backGroundId);
    }
    void HandleHeroTrial()
    {
        var tryHero = DataManager.Save.TryHero;
        if (currentSessionSave.IsTriedHero) return;
        if (!tryHero.CanTriedHero()) return;

        if (Stage == 0)
        {
            Game.Pool.GameObjectSpawner.Instance.Get("HeroTrialPoint", obj =>
            {
                obj.transform.position = GetLevelBuilder().CurrentMap().groupNpcSpawn.GetSpawnPoint(ESpawnPoint.P0).Position;
                obj.gameObject.SetActive(true);
            });
        }
    }

    //game start
    public override async UniTask StartBattle()
    {
        CameraController.Instance.Follow(GetMainActor().GetMidTransform(), Vector2.zero,true);
        await UniTask.Delay(1000, ignoreTimeScale: false, cancellationToken: cancellationToken.Token);

        if (currentRoomType == ERoomType.Normal)
        {
            StageStartPanel stageStartPanel = (StageStartPanel)await UI.PanelManager.CreateAsync(AddressableName.UIStageStartPanel);
            stageStartPanel.SetUp(Stage + 1);
            await UniTask.Delay(1000, ignoreTimeScale: false, cancellationToken: cancellationToken.Token);
            stageStartPanel.Close();

            Messenger.Broadcast<int, int>(EventKey.GameStart, Dungeon,Stage);


            await UniTask.Delay(200, ignoreTimeScale: false, cancellationToken: cancellationToken.Token);
        }
        GetMainActor().PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);

        //

    }

    async UniTask<PlayerController> PreparePlayer()
    {
        var mainWeapon = GameSceneManager.Instance.PlayerData.EquipmentHandler.GetEquipment(EEquipment.MainWeapon);
        PlayerController player = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(string.Format(AddressableName.Player,(int)(DataManager.Save.User.HeroCurrent)),Game.Pool.EPool.Pernament)).GetComponent<PlayerController>();

        WeaponBase playerWeapon = await assetLoader.LoadAsync<WeaponBase>("Weapon_"+ (mainWeapon == null ? "4" : mainWeapon.Id)).Task;
        var weaponData = new WeaponData
        {
            Weapon = playerWeapon,
            Item = mainWeapon   
        };
        //var stats = GetStatPlayer();
        var heroCurrentUnlockNotTried = DataManager.Save.User.Hero;
        var heroSaveCurrentUnlock = DataManager.Save.User.GetHero(heroCurrentUnlockNotTried);
        //stats = HeroFactory.Instance.GetHeroStatGroup(stats, DataManager.Save.User.HeroCurrent, heroSaveCurrentUnlock.Level, heroSaveCurrentUnlock.Star);
        
        
        // Add DPS change listener
        player.WeaponHandler.LoadWeapon(playerWeapon, weaponData);
        var stats = HeroFactory.Instance.GetHeroStat(heroCurrentUnlockNotTried, heroSaveCurrentUnlock.Level, heroSaveCurrentUnlock.Star);
        stats = HeroFactory.Instance.ApplyEquipment(stats, GameSceneManager.Instance.PlayerData.EquipmentHandler, out var equipmentHandlerTrialHero);
        await player.SetUp(stats);
        CalculateDPS(stats);
        var weaponEntity = DataManager.Base.Weapon.Get(mainWeapon.Id);
        //stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.FlatMul, weaponEntity.OutputDmg), this);

        player.PropertyHandler.AddProperty(EActorProperty.Vunerable,1);
        player.AnimationHandler.SetSkin("-1");
     
        return player;

    }

  

    async UniTask<Drone> PrepareDrone(string id)
    {
        //prepare drone

        drone = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(id, Game.Pool.EPool.Pernament)).GetComponent<Drone>();
        await Drone.SetUp(GetStatDrone(((Drone)drone).DroneType));
        Drone.SetActive(false);

        return (Drone)drone;
    }

    #region Add DPS Event
    private void CalculateDPS(IStatGroup stats)
    {
        var newDPS = stats.GetValue(StatKey.Dmg);
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
    public override DungeonSessionSave GetSession()
    {
        return currentSessionSave;
    }
    public override DungeonSave GetDungeonSave()
    {
        return dungeonSave;
    }
    public override DungeonEntity GetDungeonEntity()
    {
        return dungeonSpawnCurrent;
    }
    private IStatGroup GetStatPlayer()
    {
        var stats = new PlayerStat();
        stats.Copy(GameSceneManager.Instance.PlayerData.Stats);
        stats.CalculateStats();
        return stats;
    }
    private IStatGroup GetStatDrone(EDrone droneType)
    {
        var playerStats = GameSceneManager.Instance.PlayerData.Stats;
        var stats = DroneStat.Default() ;
        stats.SetBaseValue(StatKey.Dmg, playerStats.GetStat(StatKey.Dmg).Value);

        switch (droneType)
        {
            case EDrone.Bazooka:
                stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, 1), null);
                break;
            case EDrone.Gatling:
                stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, 0.3f), null);
                break;
            default:
                break;
        }

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

    public Character Drone => drone;
    #endregion
    public override void SetMainActor(ActorBase actor)
    {
        //set new player
        this.player = (Character)actor;
    }
    public override async UniTask Finish()
    {
        if (isFinished) return;
        Sound.Controller.Instance.soundData.PlayWinSFX();
        var subscription = Architecture.Get<SubscriptionService>();
        if (currentSessionSave.Mode == GameMode.Normal)
        {
            var rew = DataManager.Base.Dungeon.Get(Dungeon).GetStage(Stage);

            foreach (var reward in rew.RewardStage)
            {
                var rw = reward.Clone();
                var gold = rw.Data as ResourceData;
                string resource = "";
                double own = 0;
                double value = 0;
                if (gold != null)
                {
                    own = DataManager.Save.Resources.GetResource(gold.Resource);
                    value = gold.Value;
                    resource = gold.Resource.ToString();
                }
                var exp = rw.Data as ExpData;
                if (exp != null)
                {
                    resource = "Exp";
                    value = exp.Exp;
                    own = DataManager.Save.User.Experience;
                }

                currentSessionSave.AddLoot(rw);


                FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
                    .AddStringParam("item_category", rw.Type.ToString())
                    .AddStringParam("item_id", resource)
                    .AddStringParam("source", "campaign")
                    .AddIntParam("source_id", Stage + 1)
                    .AddFloatParam("value", (float)value)
                    .AddFloatParam("remaining_value", (float)own)
                    .AddFloatParam("total_earned_value", 0)
                    .Track();
            }
        }
        //
        //to next stage;
        Stage++;

        //
        await base.Finish();

        //dungeon finish
        if (Stage >= dungeonSpawnCurrent.Stages.Count)
        {
            Sound.Controller.Instance.StopMusic();
            //Logger.Log("DUNGEON FINISH");
            //load next stage
            var panel = await UI.PanelManager.CreateAsync<UIEndGamePanel>(AddressableName.UIEndGamePanel);
            panel.Show(EBattleResult.Win);
        }
        else
        {
            if (currentRoomType == ERoomType.Angel)
            {
                playThemeNext = true;
            }
            else
            {
                DungeonRoomEntity dungeonRoom =  dungeonSpawnCurrent.GetStage(Stage).GetRoom();
                //if next room is fountain room
                if (dungeonRoom.Map.Contains("Angel"))
                {
                    Sound.Controller.Instance.StopMusic();
                }

            }
            TrackEndGame();

            await Game.Transitioner.Controller.Instance.Trigger(GetWorldColor());
            await UniTask.Delay(250);


            await PrepareLevel();
            Game.Transitioner.Controller.Instance.Release();

            CameraController.Instance.ResetCameraSize();
            startFb.PlayFeedbacks();

            await StartBattle();
        }


    }
    protected virtual Color GetWorldColor()
    {
        return gameModeData.GetWorldColor(Dungeon);
    }

    protected virtual void TrackEndGame(int type = 1)
    {
        var now = UnbiasedTime.UtcNow - GameController.Instance.TimeStartGame;
        var totalSecond = now.TotalSeconds;
        var actor = GameController.Instance.GetMainActor();
        int remainhp = 1000;
        if (actor != null)
        {
            remainhp = (int)actor.HealthHandler.GetHealth();
        }

        int stageId = (Dungeon + 1) * 100 + Stage + 1;

        FirebaseAnalysticController.Tracker.NewEvent("battle_end")
            .AddStringParam("battle_type", currentSessionSave.Mode.ToString())
            .AddStringParam("battle_id", "1")
            .AddIntParam("stage_id", stageId)
            .AddIntParam("win", type)
            .AddIntParam("play_time", (int)totalSecond)
            .AddStringParam("hero_used", DataManager.Save.User.HeroCurrent.ToString())
            .AddIntParam("remaining_hp", remainhp)
            .AddStringParam("battle_info", JsonConvert.SerializeObject(battleSession))
            .Track();
    }

    public override async UniTask Lose()
    {
        if (isFinished) return;
        TrackEndGame(0);
        await base.Lose();

        await UniTask.Delay(1500,cancellationToken:cancellationToken.Token);
        //show end game

        var panel = await UI.PanelManager.CreateAsync<UIEndGamePanel>(AddressableName.UIEndGamePanel);
        panel.Show(EBattleResult.Lose);
    }


#if DEVELOPMENT || UNITY_EDITOR
    bool debug = false;
    string enemyId;
    string roomId;
    string buffId;
    string droneID;
    string objectID;
    [SerializeField]
    private int enemyHealth = 10000;
    List<ActorBase> enemies=new List<ActorBase>();
    private void OnGUI()
    {
        if (debug == false) return;
        GUILayout.Space(Screen.height * 0.1f);
        //if (GUILayout.Button(debug ? "Hide Debug" : "Show debug", GUILayout.Width(Screen.width * 0.1f), GUILayout.Height(Screen.height * 0.1f)))
        //{
        //    debug = !debug;
        //}
        if (debug)
        {
            //spawn enemy
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enemy Id");
            enemyId = GUILayout.TextField(enemyId, GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f));
            if (GUILayout.Button("Spawn", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                GetMainActor().PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);
                Spawn();
            }
            if (enemies.Count>0 && GUILayout.Button("DeSpawn", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                Despawn();
            }
            GUILayout.EndHorizontal();

            // buff
            GUILayout.BeginHorizontal();
            GUILayout.Label("Buff ID");
            buffId = GUILayout.TextField(buffId, GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f));
            if (GUILayout.Button("Apply Buff", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                var buffType = (EBuff)(int.Parse(buffId));
                //abilityLive.Value.AcceptAbility(index);

                Debug.Log("Cast Debug: " + buffType);
                Messenger.Broadcast(EventKey.CastBuff, FindObjectOfType<BattleGameController>().player as ActorBase, buffType);
            }

            GUILayout.EndHorizontal();


            // test room
            GUILayout.BeginHorizontal();
            GUILayout.Label("ROOM ID");
            roomId = GUILayout.TextField(roomId, GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f));
            if (GUILayout.Button("Create level", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                int room = int.Parse(roomId);
                TestSpawnRoom(room);

            }

            GUILayout.EndHorizontal();
            
            // test drone
            GUILayout.BeginHorizontal();
            GUILayout.Label("DRONE ID");
            droneID = GUILayout.TextField(droneID, GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f));
            if (GUILayout.Button("Create Drone", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                Prepare();

                async UniTask Prepare()
                {
                    drone=await PrepareDrone(droneID);
                    Drone.SetActive(true);

                    Drone.StartBehaviours();

                    Debug.Log("DRONE START: " + Drone.Stats.GetStat(StatKey.Dmg).BaseValue+ " => "+Drone.Stats.GetStat(StatKey.Dmg).Value);
                }
            }

            GUILayout.EndHorizontal();
            
            // test chest
            GUILayout.BeginHorizontal();
            GUILayout.Label("object ID");
            objectID = GUILayout.TextField(objectID, GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f));
            if (GUILayout.Button("Create ", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                Prepare();

                async UniTask Prepare()
                {
                    Game.Pool.GameObjectSpawner.Instance.Get(objectID, obj =>
                    {
                        obj.transform.position = Vector3.zero;
                        obj.gameObject.SetActive(true);
                    });
                  
            }
            }

            GUILayout.EndHorizontal();
            // test chest
            GUILayout.BeginHorizontal();
            GUILayout.Label("FINISH LEVEL");
            if (GUILayout.Button("KILL ENEMY ", GUILayout.Width(Screen.width * 0.15f), GUILayout.Height(Screen.height * 0.1f)))
            {
                ((Character)GetEnemySpawnHandler().enemies[0]).GetHit(new DamageSource(GetMainActor(), ((Character)GetEnemySpawnHandler().enemies[0]), ((Character)GetEnemySpawnHandler().enemies[0]).HealthHandler.GetMaxHP(), null),GetMainActor());
            }

            GUILayout.EndHorizontal();
        }
    }
    async UniTask TestSpawnRoom(int room)  
    {
        Game.Transitioner.Controller.Instance.Trigger();

        await UniTask.Delay(2000);

       
        await PrepareLevel(room);
        Game.Transitioner.Controller.Instance.Release();

        CameraController.Instance.ResetCameraSize();
        startFb.PlayFeedbacks();

        await StartBattle();
    }
   
    void Despawn()
    {
        foreach (var enemy in enemies)
        {
            ((Character)enemy).Dead();
        }
    }
    async UniTaskVoid Spawn()
    {
        Character enemy = null;

        enemy= await GetEnemySpawnHandler().SpawnSingle($"Enemy{enemyId}", 1, GetLevelBuilder().CurrentMap().groupNpcSpawn.GetSpawnPoint(ESpawnPoint.P0).Position,1, usePortal: true);
        if (enemy == null)
        {
            enemy = await GetEnemySpawnHandler().SpawnSingle($"{enemyId}", 1, GetLevelBuilder().CurrentMap().groupNpcSpawn.GetSpawnPoint(ESpawnPoint.P0).Position, 1, usePortal: true);

        }
        enemy.HealthHandler.SetMaxHealth(enemyHealth);
        enemy.HealthHandler.SetHealth(enemyHealth);
        enemy.HealthHandler.SetMaxHealth(enemyHealth);
        enemy.PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);

        EnemyEntity enemyEntity =null;
        if (DataManager.Base.Enemy.Dictionary.ContainsKey($"Enemy{enemyId}"))
        {
            enemyEntity = DataManager.Base.Enemy.Dictionary[$"Enemy{enemyId}"];
        }
        else
        {
            enemyEntity = DataManager.Base.Enemy.Dictionary[$"{enemyId}"];
        }
        enemy.GetTransform().localScale = Vector3.one * enemyEntity.BodySize;
        enemy.GetRigidbody().bodyType = enemyEntity.BodyType;

        if (enemyEntity.BodyType == RigidbodyType2D.Kinematic)
        {
            enemy.GetRigidbody().velocity = Vector3.zero;
            enemy.MoveHandler.Locked = true;
        }

        enemies.Add(enemy);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ((Character)GetEnemySpawnHandler().enemies[0]).GetHit(new DamageSource(GetMainActor(), ((Character)GetEnemySpawnHandler().enemies[0]), ((Character)GetEnemySpawnHandler().enemies[0]).HealthHandler.GetMaxHP(),null), GetMainActor());

        }
    }
#endif
    public override void ClearSession()
    {
        TrackEndGame(2);
        base.ClearSession();
        currentSessionSave.Clear();
    }
#if DEVELOPMENT|| UNITY_EDITOR

    internal void HideGUI()
    {
        debug = false;
    }

    internal void ShowGUI()
    {
        debug = true;
    }
#endif

    private void OnApplicationQuit()
    {
        TrackEndGame(3);
    }

    public override ActorBase GetDroneActor()
    {
        return drone;
    }
}
