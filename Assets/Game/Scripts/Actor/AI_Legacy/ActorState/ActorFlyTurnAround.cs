using Game.AI.State;
using Game.Fsm;
using Game.GameActor;
using UnityEngine;

public class ActorFlyTurnAround : ActorRunAroundState
{
    public ValueConfigSearch startTurnLeftRight;
    private bool isInit = false;
    public override void Enter()
    {
        base.Enter();
        if (!isInit)
        {
            isInit = true;
            Actor.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * startTurnLeftRight.SetId(Actor.gameObject.name).FloatValue : Actor.MoveHandler.lastMove, 1);
        }
        else
        {
            Actor.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) : Actor.MoveHandler.lastMove, 1);
        }
    }
    public override void Execute()
    {
        DetectObstacle(Actor);

        if (!Actor.MoveHandler.isMoving)
        {

            Actor.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) : Actor.MoveHandler.lastMove, 1);
        }
            
    }
    public override void DetectObstacle(ActorBase actor)
    {

        if (Time.time - time > 0.1f)
        {
            Vector3 point = ((Character)actor).frontTransform.position;
            //detect wall in front of character
            RaycastHit2D hit = Physics2D.CircleCast(point, 0.5f, actor.MoveHandler.move, 0, wallMask);

            //if there is wall
            if (hit.collider != null)
            {
                TurnBack(actor);
            }
            time = Time.time;
        }
    }
}