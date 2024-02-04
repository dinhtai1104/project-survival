using Game.GameActor;

namespace Game.Fsm
{
    public interface IState
    {
        ActorBase Actor { get; set; }
        void InitializeStateMachine();
        void Enter();
        void Exit();
        void Execute();
        void Reset();
    }
}