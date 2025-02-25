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
            GameArchitecture.GetService<IEventMgrService>().Subscribe<JoystickMovementStartEventArgs>(OnJoystickMovementStart);
            GameArchitecture.GetService<IEventMgrService>().Subscribe<JoystickMovementEventArgs>(OnJoystickMovement);
            GameArchitecture.GetService<IEventMgrService>().Subscribe<JoystickMovementEndEventArgs>(OnJoystickMovementEnd);
            GameArchitecture.GetService<IEventMgrService>().Subscribe<InputButtonSkillEventArgs>(OnInputButtonSkill);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }

        public void RemoveAllListener()
        {
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<JoystickMovementStartEventArgs>(OnJoystickMovementStart);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<JoystickMovementEventArgs>(OnJoystickMovement);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<JoystickMovementEndEventArgs>(OnJoystickMovementEnd);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<InputButtonSkillEventArgs>(OnInputButtonSkill);
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