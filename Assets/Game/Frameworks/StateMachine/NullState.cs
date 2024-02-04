using Game.GameActor;

namespace Game.Fsm
{
    public class EmptyState : IState
    {
        public static readonly IState NullState = new EmptyState();
        public ActorBase Actor { get; set; }

        public void InitializeStateMachine()
        {
        }
        private EmptyState()
        {

        }

        public bool IsActive { get { return false; } }


        public void Enter()
        {
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }

        public void Reset()
        {

        }

    }
}