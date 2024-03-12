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
        void SubscribeControl(EControlCode controlCode, UnityAction action);
        void UnsubscribeControl(EControlCode controlCode, UnityAction action);
        void InvokeControl(EControlCode controlCode);
    }
}