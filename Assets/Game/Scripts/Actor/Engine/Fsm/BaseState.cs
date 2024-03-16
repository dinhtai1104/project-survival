using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public abstract class BaseState : MonoBehaviour, IState
    {
        public UnityEvent<ActorBase> OnEnterState;
        public UnityEvent<ActorBase> OnExitState;
        public ActorBase Actor { set; get; }
        protected virtual void OnEnable()
        {
        }
        protected virtual void OnDisable()
        {
        }

        public virtual void Enter()
        {
            OnEnterState?.Invoke(Actor);
        }
        public virtual void Execute() { }

        public virtual void Exit()
        { 
            OnExitState?.Invoke(Actor);
        }

        public virtual void Reset() { }
        public virtual void InitializeStateMachine() { }
    }
}