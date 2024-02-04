using Game.GameActor;
using UnityEngine;


namespace AI.StateMachine
{
    [CreateAssetMenu(menuName = "FSM/Action/StopMovement")]
    public class StopMovementAction : Action
    {
        public override Action Init(StateMachineHandler stateMachineHandler)
        {
            StopMovementAction instance = CreateInstance<StopMovementAction>();
            return instance;
        }
        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
        }
        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
            stateMachineHandler.actor.MoveHandler.Stop();
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            base.Execute(stateMachineHandler);

        }
    }
}