using Game.GameActor;
using UnityEngine;


namespace AI.StateMachine
{
    [CreateAssetMenu (menuName = "FSM/Action/JumpBack")]
    public class JumpBackAction : Action
    {
        [SerializeField]
        private  Vector2 jumpDirection;
        [SerializeField]
        private float jumpForce;
        public override Action Init(StateMachineHandler stateMachineHandler)
        {
            JumpBackAction instance = CreateInstance<JumpBackAction>();
            instance.jumpDirection = jumpDirection;
            instance.jumpForce = jumpForce;
            return instance;
        }
        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
        }
        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
            ITarget target = stateMachineHandler.GetComponent<DetectTargetHandler>().CurrentTarget;
            int direction = target.GetPosition().x < stateMachineHandler.actor.GetPosition().x ? 1 : -1;
            stateMachineHandler.actor.MoveHandler.Move(new Vector2(-direction, 1f), 1);
            jumpDirection.x *= direction * jumpDirection.x;
            stateMachineHandler.actor.MoveHandler.Jump(jumpDirection, jumpForce);
            Debug.Log("JUMP BACK " + direction + " " + jumpDirection);
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            base.Execute(stateMachineHandler);
          
        }
    }
}