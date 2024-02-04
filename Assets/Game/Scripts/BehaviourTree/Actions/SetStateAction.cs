using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TypeReferences;

[TaskCategory("Enemy")]
public class SetStateAction : Action
{
    [SerializeField] private SharedActor Actor;
    protected virtual ClassTypeReference stateType { get; }
    public override void OnStart()
    {
        base.OnStart();
        if (Actor.Value.Machine.IsCurrentState(stateType.Type))
        {
            return;
        }
        Actor.Value.Machine.ChangeState(stateType.Type);
    }
    public override TaskStatus OnUpdate()
    {
        return base.OnUpdate();
    }
}