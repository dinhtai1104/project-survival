using Game.Fsm;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorEnemy7003PatrolState : BaseState
    {
        private Vector3 lastMove = Vector3.zero;
        [SerializeField]
        private float leftSide, rightSide;
        public override void Enter()
        {
            base.Enter();
            Vector3 dir = Vector3.right;
        
            Actor.MoveHandler.Move(dir, 1);

        }
        public override void Exit()
        {
            base.Exit();
        }


        public override void Execute()
        {
            base.Execute();

            if (Actor.GetPosition().x > rightSide)
            {
                Actor.SetPosition(new Vector3(leftSide,Actor.GetPosition().y));
            }
        
        }
    
    }
}