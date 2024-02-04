using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class TurnAroundAction : Action
{
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();
    }
    void TurnBack(ActorBase character)
    {
        ((Character)character).SetLookDirection(0, 0);
        if (character.MoveHandler.isMoving)
        {
            (character).MoveHandler.Move((character).MoveHandler.move.normalized * -1, 1);
        }
        else
        {
            (character).MoveHandler.Move((character).MoveHandler.lastMove.normalized * -1, 1);
        }
    }
    public override TaskStatus OnUpdate()
    {
        TurnBack(actor);
        return TaskStatus.Success;
    }
}
