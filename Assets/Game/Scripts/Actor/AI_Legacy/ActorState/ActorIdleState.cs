using Game.Fsm;
using Game.Handler;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorIdleState : BaseState
    {
        public override void Enter()
        {
            base.Enter();
            HealthBarHandler.Instance.Get(Actor)?.SetActive(true);
            Actor.AnimationHandler.SetIdle();
        }
    }
}