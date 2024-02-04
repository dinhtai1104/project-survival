using Game.GameActor;
using UnityEngine;

public abstract class ActorAction : MonoBehaviour, IAction<ActorBase>
{
    public abstract void Begin(ActorBase owner);

    public abstract void End(ActorBase owner);
}