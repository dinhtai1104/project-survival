using Game.Fsm;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorDeadState : BaseState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Dead State Enter");
        }
    }
}