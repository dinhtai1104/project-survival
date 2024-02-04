using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public interface IStatusEngine
{
    bool Locked { get; set; }
    void Initialize(ActorBase character);
    void Tick(float dt);
    void LateTick(float dt);
    void SetImmune<TStatus>(bool immune) where TStatus : BaseStatus;
    bool IsImmnune<TStatus>() where TStatus : BaseStatus;
    bool HasStatus<TStatus>() where TStatus : BaseStatus;
    void ClearStatus<TStatus>() where TStatus : BaseStatus;
    void ClearAllStatuses();
    UniTask<BaseStatus> AddStatus(ActorBase source, EStatus eStatus, object sourceCast);
}