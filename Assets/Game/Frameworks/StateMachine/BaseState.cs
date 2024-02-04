using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Fsm
{
    public abstract class BaseState : MonoBehaviour, IState
    {
        public ActorBase Actor { set; get; }
        private List<IStateEnterCallback> _listStateEnterCallback;
        private bool IsDead = false;
        private void OnEnable()
        {
            IsDead = false;
            _listStateEnterCallback = new List<IStateEnterCallback>(GetComponentsInChildren<IStateEnterCallback>());
        }

        public virtual void Enter() 
        {
            if (IsDead) return;
            IsDead = true;
            foreach (var cb in _listStateEnterCallback)
            {
                cb.SetActor(Actor);
                cb.Action();
            }
        }
        public virtual void Execute() { }
        public virtual void Exit() { }
        public virtual void Reset() { }
        public virtual void InitializeStateMachine() { }
    }
}