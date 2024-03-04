using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public interface IInputHandler
    {
        bool Active { set; get; }
        bool Lock { set; get; }
        bool IsHoldingAttackButton { set; get; }
        bool IsUsingJoystick { set; get; }
        Vector2 JoystickDirection { set; get; }
        float JoystickDirectionScalar { set; get; }

        void Init(Actor actor);
        void OnUpdate();
        void SubscribeControl(ControlCode controlCode, UnityAction action);
        void UnsubscribeControl(ControlCode controlCode, UnityAction action);
        void InvokeControl(ControlCode controlCode);
    }
}