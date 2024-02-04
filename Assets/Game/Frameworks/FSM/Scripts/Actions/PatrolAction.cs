using Game.GameActor;
using UnityEngine;


namespace AI.StateMachine
{
    // custom action


    [CreateAssetMenu (menuName = "FSM/Action/RunAround")]
    public class PatrolAction : Action
    {
        [SerializeField]
        private LayerMask wallMask;
        public override Action Init(StateMachineHandler stateMachineHandler)
        {
            PatrolAction instance = CreateInstance<PatrolAction>();
            instance.wallMask = wallMask;
            return instance;
        }
        public override void OnStart(StateMachineHandler stateMachineHandler)
        {
            Debug.Log(GetType().ToString() + " ONSTART");
        }
        public override void OnEnd(StateMachineHandler stateMachineHandler)
        {
            Debug.Log(GetType().ToString() + " OnEnd(");
        }
        public override void Execute(StateMachineHandler stateMachineHandler)
        {
            base.Execute(stateMachineHandler);
            DetectObstacle(stateMachineHandler.actor);
            if (!stateMachineHandler.actor.MoveHandler.isMoving)
            {
                Debug.Log("RUN AROUND hehe");
                stateMachineHandler.actor.MoveHandler.Move(new Vector2(UnityEngine.Random.Range(0, 1f) > 0.5f ? 1 : -1, 0), 1);
            }
        }
        float time = 0;
        public  void DetectObstacle(ActorBase actor)
        {
            if (Time.time - time > 0.1f)
            {
                Vector3 point = ((Character)actor).frontTransform.position;
                //detect wall in front of character
                RaycastHit2D hit = Physics2D.CircleCast(point, 0.5f, actor.MoveHandler.move, 0, wallMask);

                //if there is wall
                if (hit.collider != null)
                {
                    TurnBack(actor);
                }
                else
                {
                    //detect edge
                    hit = Physics2D.Raycast(point, Vector2.down, 1.5f, wallMask);
                    //if there is no ground
                    if (hit.collider == null)
                    {
                        TurnBack(actor);
                    }
                }
                time = Time.time;
            }


        }
        // move the character back
        void TurnBack(ActorBase character)
        {
            (character).MoveHandler.Move((character).MoveHandler.move.normalized * -1, 1);

        }

        
    }
}