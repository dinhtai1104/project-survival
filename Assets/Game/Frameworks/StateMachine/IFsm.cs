using Game.GameActor;
using System;

namespace Game.Fsm
{
    public interface IFsm
    {
        ActorBase Actor { set; get; }
        void Initialize(ActorBase actor);
        void Ticks();
        void BackToDefaultState();
        void ChangeToEmptyState();
        void SetInitialState<T>() where T : IState;
        void SetInitialState(Type T);

        void ChangeState<T>() where T : IState;
        void ChangeState(Type T);

        bool IsCurrentState<T>(bool allowBaseType = false) where T : IState;
        bool IsCurrentState(Type T);

        T GetState<T>() where T : IState;

        bool AddState(IState state);
        bool AddState<T>(IState state);
        bool AddState(Type T);

        void RemoveState<T>() where T : IState;
        void RemoveState(Type T);

        bool HasState<T>() where T : IState;
        bool HasState(Type T);

        void RemoveAllStates();
        void Reset();
    }
}