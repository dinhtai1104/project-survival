using UnityEngine;


namespace AI.StateMachine
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateMachineHandler stateMachineHandler);
    }
}