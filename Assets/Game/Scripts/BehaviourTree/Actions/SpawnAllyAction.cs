using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;
using Game.GameActor;

public class SpawnAllyAction : Action
{
    public string spawnId;
    public ValueConfigSearch total;
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();

    }
    public override TaskStatus OnUpdate()
    {
        SpawnAlly().Forget();
        return TaskStatus.Success;
    }
    async UniTaskVoid SpawnAlly()
    {
        Messenger.Broadcast(EventKey.EnemyAllySpawn);

        for (int i = 0; i < total.IntValue; i++)
        {
            await Game.Controller.Instance.gameController.GetEnemySpawnHandler().SpawnSingle(spawnId, (int)actor.Stats.GetValue(StatKey.Level), actor.GetPosition());
        }
    }
}