using Game.GameActor;
using UnityEngine;


namespace AI.StateMachine
{
    [CreateAssetMenu(menuName = "FSM/Action/LookAtTarget")]
    public class LookAtTargetAction : Action
    {
        public override Action Init(StateMachineHandler stateMachineHandler)
        {
            LookAtTargetAction instance = CreateInstance<LookAtTargetAction>();
            return instance;
        }
        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
        }
        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
            ITarget target = stateMachineHandler.GetComponent<DetectTargetHandler>().CurrentTarget;
            int direction = target.GetPosition().x > stateMachineHandler.actor.GetPosition().x ? 1 : -1;
            ((Character)stateMachineHandler.actor).SetFacing(direction);

        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            base.Execute(stateMachineHandler);

        }
    }
}