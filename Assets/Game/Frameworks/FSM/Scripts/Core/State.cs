using System.Collections.Generic;
using UnityEngine;

namespace AI.StateMachine
{
    [CreateAssetMenu (menuName ="FSM/State")]
    public class State : BaseState
    {
        public List<Action> actions = new List<Action>();
        public List<Transition> transitions = new List<Transition>();

        public override BaseState Init(StateMachineHandler stateMachineHandler)
        {
            State instance = CreateInstance<State>();
            instance.Clone(this);
            foreach(var action in actions)
            {
                instance.actions.Add(action.Init(stateMachineHandler));
            }
            foreach (var transition in transitions)
            {
                instance.transitions.Add(transition);
            }
            return instance;
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            foreach (var action in actions)
                action.Execute(stateMachineHandler);

            foreach (var transition in transitions)
                transition.Execute(stateMachineHandler);
        }

        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
            Debug.Log("STATE START " + name);
            foreach(var action in actions)
            {
                action.OnStart(stateMachineHandler);
            }
        }

        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
            foreach (var action in actions)
            {
                action.OnEnd(stateMachineHandler);
            }
        }
    } 
}