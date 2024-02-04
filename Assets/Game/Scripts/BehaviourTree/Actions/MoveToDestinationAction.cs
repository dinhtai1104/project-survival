using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class MoveToDestinationAction : Action
{
    [SerializeField] private ActorBase actor;
    [SerializeField] private SharedVector2 destination;

    public  Vector3 lastPosition;
    float timeCheckPosition = 2;

    float timer = 0;

    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();
        timer = 0;
        lastPosition = Vector2.zero;
    }
    public override TaskStatus OnUpdate()
    {
        actor.MoveHandler.Move((destination.Value-(Vector2)actor.GetPosition()).normalized,1);
        if (timer <timeCheckPosition)
        {
            timer += Time.deltaTime;
        }
        else
        {
            //bi ket
            if (Vector2.Distance(lastPosition, actor.GetPosition()) < 0.5f)
            {
                 //Logger.Log("STUCK: " + Vector2.Distance(actor.GetPosition(), lastPosition));
                return TaskStatus.Success;
            }
            lastPosition = actor.GetPosition();
        }

        //Logger.Log("ARRIVE: " + Vector2.Distance(actor.GetPosition(), destination.Value));
        return Vector2.Distance(actor.GetPosition(), destination.Value) < 1f ? TaskStatus.Success : TaskStatus.Running;
    }
}
