using Game.Fsm;
using Game.GameActor;
using System;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorRunAroundState : BaseState
    {
        [SerializeField]
        protected LayerMask wallMask;
        private Vector3 lastMove = Vector3.zero;
        public bool detectEdge = true;
        public override void Enter()
        {
            base.Enter();

            Vector3 dir = Vector3.right;
            dir = Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right * (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) : Actor.MoveHandler.lastMove;

            var rch = Physics2D.Raycast(Actor.GetMidPos(), dir, 1.5f, wallMask);
            if (rch.collider)
            {
                dir.x *= -1;
            }
            Actor.MoveHandler.lastMove = dir;
            isStanding = false;
        }
        public override void Exit()
        {
            base.Exit();
        }

        private Vector2 lastPos;

        public override void Execute()
        {
            base.Execute();
            if (Actor.MoveHandler.isGrounded)
            {
                DetectObstacle(Actor);

                if (!Actor.MoveHandler.isMoving)
                {
                    Actor.MoveHandler.isMoving = true;

                }
                Actor.MoveHandler.Move(Actor.MoveHandler.lastMove, 1);
                lastMove = Actor.MoveHandler.lastMove;
                Actor.AnimationHandler.SetRun();
            }
            lastPos = Actor.transform.position;
        }
        protected float time = 0;
        protected float timeStanding = 0;
        protected bool isStanding = false;
        public virtual void DetectObstacle(ActorBase actor)
        {
            if (Time.time - time > 0.1f)
            {
                Vector3 point = ((Character)actor).frontTransform.position;
                //detect wall in front of character
                RaycastHit2D hit = Physics2D.CircleCast(point, 0.5f * actor.transform.localScale.x, actor.MoveHandler.lastMove, 0, wallMask);
                    
                //if there is wall
                if (hit.collider != null)
                {
                    TurnBack(actor);
                }
                else
                {
                    hit = Physics2D.CircleCast(point, 0.5f * actor.transform.localScale.x, -actor.MoveHandler.lastMove, 0, wallMask);
                    if (hit.collider != null)
                    {
                        TurnBack(actor);
                    }
                    else
                    {
                        if (detectEdge)
                        {
                            //detect edge
                            hit = Physics2D.Raycast(point, Vector2.down * actor.transform.localScale.x, 1.5f, wallMask);
                            //if there is no ground
                            if (hit.collider == null)
                            {
                                TurnBack(actor);
                            }
                        }
                    }
                }
                time = Time.time;
            }
        }
        // move the character back
        protected void TurnBack(ActorBase character)
        {
            (character).MoveHandler.Move((character).GetLookDirection() * -1, 1);

        }
    }
}