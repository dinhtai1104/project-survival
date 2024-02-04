using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class CollideWithWallDecision : Conditional
{
    ActorBase actor;
    public bool detectEdge=true;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override TaskStatus OnUpdate()
    {
        return DetectObstacle(actor) ? TaskStatus.Success : TaskStatus.Running;
    }
    [SerializeField]
    Vector3 offsetLeft = new Vector3(-2, 0), offsetRight = new Vector3(2, 0);
    [SerializeField]
    private float groundCheckOffset = 1;
    [SerializeField]
    private LayerMask wallMask;
    public bool DetectObstacle(ActorBase actor)
    {
        Vector3 point = ((Character)actor).GetMidTransform().position;
        //detect wall in front of character
        RaycastHit2D wallHit = Physics2D.CircleCast(point + (actor.MoveHandler.move.normalized.x < 0 ? offsetLeft : offsetRight), 0.5f, (actor.MoveHandler.move.normalized.x < 0 ? offsetLeft : offsetRight), 0, wallMask);

        //if there is wall
        if (wallHit.collider != null)
        {
            return true;
        }
        else
        {
            if (actor.MoveHandler.isGrounded && detectEdge)
            {
                //detect edge
                RaycastHit2D groundHit = Physics2D.Raycast(point + (Vector3)actor.MoveHandler.move.normalized * groundCheckOffset, Vector2.down, 1.5f, wallMask);
                //if there is no ground
                if (groundHit.collider == null)
                {
                    return true;
                }
            }
        }

        return false;


    }
}
