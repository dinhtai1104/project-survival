namespace Engine
{
    public interface IState
    {
        Actor Actor { get; set; }
        void InitializeStateMachine();
        void Enter();
        void Exit();
        void Execute();
        void Reset();
    }
}