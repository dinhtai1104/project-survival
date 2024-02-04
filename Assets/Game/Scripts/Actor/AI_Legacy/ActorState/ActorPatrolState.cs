using Game.Fsm;
using Game.GameActor;
using System;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorPatrolState : BaseState
    {
        [SerializeField]
        private LayerMask wallMask;

        public bool detectEdge = true;
        public override void Enter()
        {
            base.Enter();
            Actor.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right : Actor.MoveHandler.lastMove, 1);

        }
        public override void Exit()
        {
            base.Exit();
        }

  

        public override void Execute()
        {
            base.Execute();
            //DetectObstacle(Actor);
            if (!Actor.MoveHandler.isMoving)
            {
                Actor.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) : Actor.MoveHandler.lastMove, 1);
            }


        }
        float time = 0;
        [SerializeField]
        Vector3 offsetLeft=new Vector3(-2,0), offsetRight= new Vector3(2, 0);
        [SerializeField]
        private float groundCheckOffset = 1;
        public void DetectObstacle(ActorBase actor)
        {
            if (Time.time - time > 0.04f)
            {
                Vector3 point = ((Character)actor).GetMidTransform().position;
                //detect wall in front of character
                RaycastHit2D wallHit= Physics2D.CircleCast(point+(actor.MoveHandler.move.normalized.x<0?offsetLeft:offsetRight) , 0.5f, (actor.MoveHandler.move.normalized.x < 0 ? offsetLeft : offsetRight), 0, wallMask);
                    
                //if there is wall
                if (wallHit.collider != null )
                {
                    TurnBack(actor);
                }
                else
                {
                    if (actor.MoveHandler.isGrounded && detectEdge)
                    {
                        //detect edge
                        RaycastHit2D groundHit = Physics2D.Raycast(point+(Vector3)actor.MoveHandler.move.normalized*groundCheckOffset, Vector2.down, 1.5f, wallMask);
                        //if there is no ground
                        if (groundHit.collider == null)
                        {
                            TurnBack(actor);
                        }
                    }
                }
                time = Time.time;
            }


        }
        // move the character back
        void TurnBack(ActorBase character)
        {
            (character).MoveHandler.Move((character).MoveHandler.move.normalized * -1, 1);

        }
    }
}