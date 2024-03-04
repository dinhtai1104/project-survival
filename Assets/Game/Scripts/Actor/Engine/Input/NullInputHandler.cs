using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public class NullInputHandler : IInputHandler
    {
        public NullInputHandler()
        {
        }

        public bool Active
        {
            set { }
            get { return false; }
        }

        public bool Lock
        {
            set { }
            get { return true; }
        }

        public bool IsHoldingAttackButton
        {
            set { }
            get { return false; }
        }

        public bool IsUsingJoystick
        {
            set { }
            get { return false; }
        }

        public Vector2 JoystickDirection
        {
            set { }
            get { return Vector2.zero; }
        }

        public float JoystickDirectionScalar
        {
            set { }
            get { return 0f; }
        }

        public void Init(Actor actor)
        {
        }

        public void InvokeControl(ControlCode controlCode)
        {
        }

        public void SubscribeControl(ControlCode controlCode, UnityAction action)
        {
        }

        public void OnUpdate()
        {
        }

        public void UnsubscribeControl(ControlCode controlCode, UnityAction action)
        {
        }
    }
}