using UnityEngine;


namespace AI.StateMachine
{
    [CreateAssetMenu (menuName = "FSM/Action/AttackTarget")]
    public class AttackTargetAction : Action
    {
        public override Action Init(StateMachineHandler stateMachineHandler)
        {
            AttackTargetAction instance = CreateInstance<AttackTargetAction>();
            return instance;
        }
        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
        }
        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            base.Execute(stateMachineHandler);
            //DetectTargetHandler detectTargetHandler = stateMachineHandler.actor.GetComponent<DetectTargetHandler>();
            //if (detectTargetHandler.CurrentTarget != null)
            //{
            //    stateMachineHandler.actor.AttackHandler.Trigger(detectTargetHandler.CurrentTarget);
            //}
        }
    }
}