using UnityEngine;


namespace AI.StateMachine
{ 
    [System.Serializable]
    public  class Transition
    {
        public Decision actionDecision;
        public BaseState trueState,falseState;
        
        public void Execute(StateMachineHandler stateMachineHandler)
        {
            if (actionDecision.Decide(stateMachineHandler))
            {
                stateMachineHandler.CurrentState = trueState;
            }
            else
            {
                stateMachineHandler.CurrentState = falseState;
            }
        }
    }
}