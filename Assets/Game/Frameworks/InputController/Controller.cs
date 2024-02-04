using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Skill;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace InputController
{
    public class Controller : MonoBehaviour
    {
        public bool canTrigger = true;
        protected List<ControlAction> actions=new List<ControlAction>();
        [SerializeField]
        private UnityEngine.InputSystem.InputAction input;
        public void Register(ControlAction onAction)
        {
            this.actions.Add( onAction);
            gameObject.SetActive(true);
        }
        public void Deregister(ControlAction onAction)
        {
            this.actions.Remove(onAction);
        }
        private void OnEnable()
        {
            input.Enable();
            input.started += Input_started;
            input.performed += Input_performed;
            input.canceled += Input_canceled; ;
        }
        private void OnDisable()
        {
            input.started -= Input_started;
            input.performed -= Input_performed;
            input.canceled -= Input_canceled; ;
            input.Disable();
        }
        private void OnDestroy()
        {
            input.started -= Input_started;
            input.performed -= Input_performed;
            input.canceled -= Input_canceled; ;
            input.Disable();
        }
        private void Input_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            foreach(var onAction in actions)
            {
                onAction?.onReleased?.Invoke();

            }
        }

        private void Input_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            foreach (var onAction in actions)
            {
                onAction?.onTriggered?.Invoke();
            }

        }

        private void Input_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
        }

    }

    public class ControlAction
    {
        public Sprite texture;
        public System.Action onTriggered, onReleased;
        public CoolDownConfig coolDownConfig;
        public TrackSkillConfig trackSkillTrigger;
     

        public ControlAction(Action onTriggered, Action onReleased, CoolDownConfig coolDownConfig=null, TrackSkillConfig trackSkillTrigger =null,Sprite texture=null)
        {
            this.texture = texture;
            this.onTriggered = onTriggered;
            this.onReleased = onReleased;

            this.coolDownConfig = coolDownConfig;
            this.trackSkillTrigger = trackSkillTrigger;
        }
    }
    public class TrackSkillConfig
    {
        public ActorBase actor;
        public bool trackSkillTrigger = false;

        public TrackSkillConfig(ActorBase actor, bool trackSkillTrigger)
        {
            this.actor = actor;
            this.trackSkillTrigger = trackSkillTrigger;
        }
    }
    public class CoolDownConfig 
    {
        public Condition triggerCoolDownCondition;
        public Condition deactiveCondition;
        public ISkill skill;

        public CoolDownConfig(Condition triggerCoolDownCondition,Condition deactiveCondition, ISkill skill)
        {
            this.triggerCoolDownCondition = triggerCoolDownCondition;
            this.deactiveCondition = deactiveCondition;
            this.skill = skill;
        }

        public async UniTask CheckForCoolDown(Action setCoolDown,CancellationTokenSource cancellation)
        {
            await UniTask.WaitUntil(() => triggerCoolDownCondition.Check(), cancellationToken: cancellation.Token);
            setCoolDown?.Invoke();
        }
        public async UniTask CheckForDeactive(Action deactive, CancellationTokenSource cancellation)
        {
            await UniTask.WaitUntil(() => deactiveCondition.Check(), cancellationToken: cancellation.Token);
            deactive?.Invoke();
        }
    }

}