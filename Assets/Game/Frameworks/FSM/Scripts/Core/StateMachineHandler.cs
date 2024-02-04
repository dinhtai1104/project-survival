using Game.GameActor;
using UnityEngine;


namespace AI.StateMachine
{
    public class StateMachineHandler:MonoBehaviour
    {
        public bool active = false;
        public Game.GameActor.ActorBase actor;

        //start this state on init 
        [SerializeField]
        private BaseState initState;

        //selected state
        private BaseState currentState;

        public BaseState CurrentState { get => currentState; 
            set 
            {
              
                if (value==null ||(currentState!=null &&currentState.CompareTo(value)) || value is RemainInState) return;
                if (currentState != null)
                {
                    currentState.OnEnd(this);
                }
                Debug.Log(value == null);
                Debug.Log(value.name);
                currentState = value.Init(this);
                currentState.OnStart(this);    
            } 
        }

        void OnEnable()
        {
            actor = GetComponent<ActorBase>();
            CurrentState = initState;
        }

        //process every frame
        private void Update()
        {
            if (!active) return;
            Execute();
        }
        public void Execute()
        {
            if(CurrentState!=null)
            CurrentState.Execute(this);
        }
    }
}