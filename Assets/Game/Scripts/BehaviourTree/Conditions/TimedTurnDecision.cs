using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class TimedTurnDecision : Conditional
{
    ActorBase actor;

    public SharedFloat turnTime;
    [SerializeField]
    float time = 0;
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
        if (time < turnTime.Value)
        {
            time += Time.deltaTime;
            return TaskStatus.Running;
        }
        else
        {
            time = 0;
            return TaskStatus.Success;
        }
    }
}
