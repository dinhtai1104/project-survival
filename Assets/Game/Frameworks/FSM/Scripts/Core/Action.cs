using UnityEngine;


namespace AI.StateMachine
{
    public abstract class Action : ScriptableObject
    { 
        public virtual Action Init(StateMachineHandler stateMachineHandler)
        {
            Action instance = CreateInstance<Action>();
            return instance;
        }
        public abstract void OnStart(StateMachineHandler stateMachineHandler);
        public abstract void OnEnd(StateMachineHandler stateMachineHandler);
        public virtual void Execute(StateMachineHandler stateMachineHandler)
        {
            
        }

    }
}