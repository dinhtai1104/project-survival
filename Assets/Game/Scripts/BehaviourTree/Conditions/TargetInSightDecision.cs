using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;

public class TargetInSightDecision : Conditional
{
    ActorBase actor;
    DetectTargetHandler sensor;
    public float radiusScan = 0;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
        sensor = actor.Sensor;
    }
    public override TaskStatus OnUpdate()
    {
        var target = sensor.CurrentTarget;
        if (radiusScan > 0)
        {
            if (target == null) return TaskStatus.Running;
            if (GameUtility.GameUtility.GetRange(target.GetTransform(), actor.GetTransform()) < radiusScan)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        return sensor.CurrentTarget != null ? TaskStatus.Success : TaskStatus.Running;
    }
}
