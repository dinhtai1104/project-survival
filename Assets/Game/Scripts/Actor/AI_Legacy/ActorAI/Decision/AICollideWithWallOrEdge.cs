using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AICollideWithWallOrEdge", menuName = "AI/AICollideWithWallOrEdge")]
    public class AICollideWithWallOrEdge : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return DetectObstacle(actor);
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
                TurnBack(actor);
                return true;
            }
            else
            {
                if (actor.MoveHandler.isGrounded)
                {
                    //detect edge
                    RaycastHit2D groundHit = Physics2D.Raycast(point + (Vector3)actor.MoveHandler.move.normalized * groundCheckOffset, Vector2.down, 1.5f, wallMask);
                    //if there is no ground
                    if (groundHit.collider == null)
                    {
                        TurnBack(actor);
                        return true;
                    }
                }
            }

            return false;
               

        }
        void TurnBack(ActorBase character)
        {
            (character).MoveHandler.Move((character).MoveHandler.move.normalized * -1, 1);

        }
    }
}