using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;


[TaskCategory ("Enemy")]
public class ChaseAvailableDecision : Conditional
{
    public SharedFloat chaseTimer;
    public SharedBool isChasing;
    public ValueConfigSearch ChaseCoolDown ;
    public float chaseCoolDown = 5;
    public override void OnAwake()
    {
        base.OnAwake();
        chaseCoolDown = ChaseCoolDown.SetId(GetComponent<ActorBase>().gameObject.name).FloatValue;
    }
    public override void OnStart()
    {
        base.OnStart();
   
    }

    public override TaskStatus OnUpdate()
    {
        if (isChasing.Value ||!isChasing.Value && chaseTimer.Value >= chaseCoolDown)
        {
            chaseTimer.Value = 0;
            return TaskStatus.Success;

        }
        chaseTimer.Value += Time.deltaTime;

        return TaskStatus.Running;

    }


}
