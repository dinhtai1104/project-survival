using BehaviorDesigner.Runtime;
using Game.GameActor;

[System.Serializable]
public class SharedActor : SharedVariable<ActorBase>
{
    public static implicit operator SharedActor(ActorBase value) { return new SharedActor { Value = value }; }
}
