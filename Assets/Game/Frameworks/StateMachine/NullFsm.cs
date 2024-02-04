using Game.GameActor;
using System;

namespace Game.Fsm
{
    public class NullFsm : IFsm
    {
        public ActorBase Actor { set; get; }

        public bool AddState(IState state)
        {
            return false;
        }

        public bool AddState<T>(IState state)
        {
            return false;
        }

        public bool AddState(Type T)
        {
            return false;
        }

        public void BackToDefaultState()
        {
        }

        public void ChangeState<T>() where T : IState
        {
        }

        public void ChangeState(Type T)
        {
        }

        public void ChangeToEmptyState()
        {
        }

        public T GetState<T>() where T : IState
        {
            return default;
        }

        public bool HasState<T>() where T : IState
        {
            return false;
        }

        public bool HasState(Type T)
        {
            return false;
        }

        public void Initialize(ActorBase actor)
        {
        }

        public bool IsCurrentState<T>(bool allowBaseType = false) where T : IState
        {
            return false;
        }

        public bool IsCurrentState(Type T)
        {
            return false;
        }

        public void RemoveAllStates()
        {
        }

        public void RemoveState<T>() where T : IState
        {
        }

        public void RemoveState(Type T)
        {
        }

        public void Reset()
        {
        }

        public void SetInitialState<T>() where T : IState
        {
        }

        public void SetInitialState(Type T)
        {
        }

        public void Ticks()
        {
        }
    }
}