using UnityEngine;

namespace Engine
{
    public class PlayerInput : BaseInputHandler
    {
        private bool _isHolding;

        public override bool IsHoldingAttackButton => _isHolding;

        public override void Init(Actor actor)
        {
            base.Init(actor);
            Messenger.AddListener(EventKey.InputJoystickStart, OnJoystickMovementStart);
            Messenger.AddListener(EventKey.InputJoystickEnd, OnJoystickMovementEnd);
            Messenger.AddListener<Vector2>(EventKey.InputJoystickMovement, OnJoystickMovement);
            Messenger.AddListener(EventKey.InputDash, OnDash);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }

        public void RemoveAllListener()
        {
            Messenger.RemoveListener(EventKey.InputJoystickStart, OnJoystickMovementStart);
            Messenger.RemoveListener(EventKey.InputJoystickEnd, OnJoystickMovementEnd);
            Messenger.RemoveListener<Vector2>(EventKey.InputJoystickMovement, OnJoystickMovement);
            Messenger.RemoveListener(EventKey.InputDash, OnDash);
        }

#if UNITY_EDITOR
        private void Update()
        {
            var dash = Input.GetKeyDown(KeyCode.Space);
            if (dash) OnDash();
        }
#endif

        private void OnJoystickMovementStart()
        {
            IsUsingJoystick = true;
        }

        private void OnJoystickMovement(Vector2 dir)
        {
            if (!Active) return;
            var normalizedDir = dir.normalized;
            JoystickDirection = normalizedDir;
            InvokeControl(ControlCode.Move);
        }

        private void OnJoystickMovementEnd()
        {
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }

        private void OnDash()
        {
            if (!Active) return;
            InvokeControl(ControlCode.Dash);
        }
    }
}