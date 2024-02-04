using UnityEngine;

namespace AI.StateMachine
{

    [CreateAssetMenu (menuName = "Transition/CollideWithTarget")]
    public class CollideWithTarget : Decision
    { 
        public override bool Decide(StateMachineHandler stateMachineHandler)
        {
            DetectTargetHandler detectTargetHandler = stateMachineHandler.actor.GetComponent<DetectTargetHandler>();

            if (detectTargetHandler.CurrentTarget != null)
            {
                return true;
            }

            return false;

        }
    }
}