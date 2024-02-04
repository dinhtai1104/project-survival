using UnityEngine;


namespace AI.StateMachine
{
    [CreateAssetMenu (menuName ="FSM/RemainInState")]
    public  class RemainInState : BaseState
    {
        public override BaseState Init(StateMachineHandler stateMachineHandler)
        {
            return this;
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
        }

        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
        }

        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
        }
    }
}