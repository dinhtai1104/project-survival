using Assets.Game.Scripts.Events;
using Core;
using UnityEngine;

namespace Engine
{
    public class PlayerInput : BaseInputHandler
    {
        private bool _isHolding;

        public override bool IsHoldingAttackButton => _isHolding;

        public override void Init(ActorBase actor)
        {
            base.Init(actor);
            Architecture.Get<EventMgr>().Subscribe<JoystickMovementStartEventArgs>(OnJoystickMovementStart);
            Architecture.Get<EventMgr>().Subscribe<JoystickMovementEventArgs>(OnJoystickMovement);
            Architecture.Get<EventMgr>().Subscribe<JoystickMovementEndEventArgs>(OnJoystickMovementEnd);
            Architecture.Get<EventMgr>().Subscribe<InputButtonSkillEventArgs>(OnInputButtonSkill);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }

        public void RemoveAllListener()
        {
            Architecture.Get<EventMgr>().Unsubscribe<JoystickMovementStartEventArgs>(OnJoystickMovementStart);
            Architecture.Get<EventMgr>().Unsubscribe<JoystickMovementEventArgs>(OnJoystickMovement);
            Architecture.Get<EventMgr>().Unsubscribe<JoystickMovementEndEventArgs>(OnJoystickMovementEnd);
            Architecture.Get<EventMgr>().Unsubscribe<InputButtonSkillEventArgs>(OnInputButtonSkill);
        }

#if UNITY_EDITOR
        private void Update()
        {
            var dash = Input.GetKeyDown(KeyCode.Space);
            if (dash)
            {
                InvokeControl(EControlCode.Dash);
            }
        }
#endif

        private void OnJoystickMovementStart(object sender, IEventArgs e)
        {
            IsUsingJoystick = true;
        }

        private void OnJoystickMovement(object sender, IEventArgs e)
        {
            if (!Active) return;
            var evt = e as JoystickMovementEventArgs;
            var normalizedDir = evt.m_Direction.normalized;
            JoystickDirection = normalizedDir;
            InvokeControl(EControlCode.Move);
        }

        private void OnJoystickMovementEnd(object sender, IEventArgs e)
        {
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }

        private void OnInputButtonSkill(object sender, IEventArgs e)
        {
            if (!Active) return;
            var evt = e as InputButtonSkillEventArgs;
            InvokeControl(evt.ControlCode);
        }
    }
}