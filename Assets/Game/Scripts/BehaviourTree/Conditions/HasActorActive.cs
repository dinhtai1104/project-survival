using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasActorActive : HasConditional
{
    [SerializeField] private SharedActor Actor;

    public override TaskStatus OnUpdate()
    {
        var active = Actor.Value.IsActived;

        if (active)
        {
            return TaskStatus.Success;
        }

        return base.OnUpdate();
    }
}