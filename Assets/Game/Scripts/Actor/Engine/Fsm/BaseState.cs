using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public abstract class BaseState : MonoBehaviour, IState
    {
        public ActorBase Actor { set; get; }
        protected virtual void OnEnable()
        {
        }
        protected virtual void OnDisable()
        {
        }

        public virtual void Enter()
        {
        }
        public virtual void Execute() { }
        public virtual void Exit() { }
        public virtual void Reset() { }
        public virtual void InitializeStateMachine() { }
    }
}