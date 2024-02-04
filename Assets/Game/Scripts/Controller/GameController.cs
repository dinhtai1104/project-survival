using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using Game.BuffCard;
using Game.GameActor;
using Game.Level;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public abstract class GameController : MonoSingleton<GameController>
{
    public delegate void OnStageStart();
    public static OnStageStart onStageStart;


    public delegate void OnStageEnd();
    public static OnStageEnd onStageEnd;

    public delegate void OnStageReady(int mode,int dungeonId,int stageId,EDungeonEvent eventType);
    public static OnStageReady onStageReady;


    public GameModeData gameModeData;
    public bool isReady = false,isFinished=false,isStageCleared;
    public int totalCoin = 0;
    protected CancellationTokenSource cancellationToken;
    public DateTime TimeStartGame;

    protected async UniTask Start()
    {
        TimeStartGame = UnbiasedTime.UtcNow;
        Messenger.Broadcast(EventKey.GameLoaded, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
        }
        //Destroy();
        ActorBase.onDie -= OnDie;

    }
    protected virtual void OnApplicationPause(bool pause)
    {

    }

    protected virtual void OnDisable()
    {
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
        }
        ActorBase.onDie -= OnDie;

    }
    protected virtual void OnEnable()
    {
        cancellationToken = new CancellationTokenSource();
        ActorBase.onDie -= OnDie;
        ActorBase.onDie += OnDie;

    }
    public virtual void Clear()
    {
    }
  
    public virtual void Destroy() 
    { 
        Addressables.ReleaseInstance(gameObject);
     
    }

    public virtual void OnMainPlayerReviveResult(bool success)
    {
        if (!success)
        {
            Lose();
        }
    }
    public virtual void SetMainActor(ActorBase actor)
    {

    }
    public virtual async UniTask SetPlaySession(DungeonSessionSave save) { }
    public virtual async UniTask Initialize() 
    {
        //

        await PrepareLevel();
        await StartBattle();


    }
    public abstract  UniTask PrepareLevel(int room=-1);
    public abstract  UniTask StartBattle();

    public abstract LevelBuilderBase GetLevelBuilder();
    public abstract EnemySpawnHandler GetEnemySpawnHandler();
    public abstract ActorBase GetMainActor();
    public virtual ActorBase GetDroneActor() { return null; }

    public virtual async UniTask Finish()
    {
        if (isFinished) return;
        isFinished = true;
    }
    public virtual async UniTask Lose()
    {
        if (isFinished) return;
        isFinished = true;
    }

    protected virtual void OnDie(ActorBase pointBase, ActorBase damageSource) 
    {
       
    }

    public virtual void ClearSession()
    {
    }

    public virtual DungeonSessionSave GetSession()
    {
        return null;
    }

    public virtual DungeonSave GetDungeonSave()
    {
        return null;
    }
    public virtual DungeonEntity GetDungeonEntity() { return null; }

    public virtual void RerollBuff(string reroll)
    {
    }
}
