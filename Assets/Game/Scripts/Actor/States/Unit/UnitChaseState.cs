using Assets.Game.Scripts.Actor.States.Common;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Unit
{
    public class UnitChaseState : BaseActorState
    {
        [SerializeField] private string m_AnimationChase;
        private Vector3 m_LastPos;
        public override void Enter()
        {
            base.Enter();
            Actor.Animation.EnsurePlay(0, m_AnimationChase, true);
        }
        public override void Execute()
        {
            base.Execute();
            var target = Actor.TargetFinder.CurrentTarget;
            if (target != null)
            {
                var dir = target.CenterPosition - Actor.CenterPosition;
                Actor.Movement.MoveDirection(dir.normalized);
            }
            else
            {
                ToIdleState();
            }
            m_LastPos = Actor.CenterPosition;
        }
        protected override void ToIdleState()
        {
            Actor.Fsm.ChangeState<UnitIdleState>();
        }
    }
}
