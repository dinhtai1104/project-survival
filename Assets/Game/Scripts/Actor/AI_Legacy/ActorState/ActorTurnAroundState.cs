using Game.Fsm;
using Game.GameActor;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorTurnAroundState : BaseState
    {
        public override void Enter()
        {
            base.Enter();
            TurnBack(Actor);
        }
        public override void Exit()
        {
            base.Exit();
        }



        public override void Execute()
        {
            base.Execute();
        }
    
        // move the character back
        void TurnBack(ActorBase character)
        {
            character.MoveHandler.Move(Actor.MoveHandler.lastMove.sqrMagnitude == 0 ? Vector2.right  : Actor.MoveHandler.lastMove, 1);

        }
    }
}