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

        public void Init(ActorBase actor)
        {
        }

        public void InvokeControl(EControlCode controlCode)
        {
        }

        public void SubscribeControl(EControlCode controlCode, UnityAction action)
        {
        }

        public void OnUpdate()
        {
        }

        public void UnsubscribeControl(EControlCode controlCode, UnityAction action)
        {
        }
    }
}