using UnityEngine;

namespace AI.StateMachine
{

    [CreateAssetMenu (menuName ="Transition/TargetInSight")]
    public class TargetInSight : Decision
    {
        public float range;
        public override bool Decide(StateMachineHandler stateMachineHandler)
        {
            DetectTargetHandler detectTargetHandler = stateMachineHandler.actor.GetComponent<DetectTargetHandler>();

            if (detectTargetHandler.CurrentTarget != null && Vector2.Distance(stateMachineHandler.actor.GetPosition(),detectTargetHandler.CurrentTarget.GetPosition())<range)
            {
                return true;
            }

            return false;

        }
    }
}