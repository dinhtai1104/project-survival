using Cysharp.Threading.Tasks;
using Game.GameActor;

public class NullStatusEngine : IStatusEngine
{
    public bool Locked { set; get; }

    public bool AddStatus<TStatus>(ActorBase source, BaseStatus status)
    {
        return false;
    }

    public void AddStatus(ActorBase source, EStatus eStatus)
    {
        return;
    }

    public void ClearAllStatuses()
    {
    }

    public void ClearStatus<TStatus>() where TStatus : BaseStatus
    {
    }

    public int CountStatus<TStatus>() where TStatus : BaseStatus
    {
        return 0;
    }

    public bool HasStatus<TStatus>() where TStatus : BaseStatus
    {
        return false;
    }

    public void Initialize(ActorBase character)
    {
    }

    public bool IsImmnune<TStatus>() where TStatus : BaseStatus
    {
        return false;
    }

    public void LateTick(float dt)
    {
    }

    public void SetImmune<TStatus>(bool immune) where TStatus : BaseStatus
    {
    }

    public void Tick(float dt)
    {
    }

    async UniTask<BaseStatus> IStatusEngine.AddStatus(ActorBase source, EStatus eStatus, object sourceCast)
    {
        await UniTask.Yield();
        return null;
    }
}