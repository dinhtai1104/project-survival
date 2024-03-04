using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Engine
{
    public abstract class BaseInputHandler : MonoBehaviour, IInputHandler
    {
        private Dictionary<ControlCode, UnityEvent> _listenerLookup;
        private IInputHandler _controlStrategy;
        private bool _active;

        [ShowInInspector]
        public virtual bool Active
        {
            set
            {
                if (_active == value) return;
                _active = value;

                if (_active)
                {
                    OnActivate();
                }
                else
                {
                    OnDeactivate();
                }
            }
            get => _active;
        }

        [ShowInInspector] public virtual bool Lock { set; get; }

        [ShowInInspector] public virtual bool IsHoldingAttackButton { set; get; }
        public virtual bool IsUsingJoystick { set; get; }
        [ShowInInspector] public virtual Vector2 JoystickDirection { set; get; }
        [ShowInInspector] public virtual float JoystickDirectionScalar { set; get; }

        public virtual void Init(Actor actor)
        {
            _listenerLookup = new Dictionary<ControlCode, UnityEvent>();
        }

        public virtual void InvokeControl(ControlCode controlCode)
        {
            if (_listenerLookup.ContainsKey(controlCode)) _listenerLookup[controlCode].Invoke();
        }

        public virtual void SubscribeControl(ControlCode controlCode, UnityAction action)
        {
            if (_listenerLookup.ContainsKey(controlCode))
            {
                _listenerLookup[controlCode].AddListener(action);
            }
            else
            {
                var listener = new UnityEvent();
                listener.AddListener(action);
                _listenerLookup.Add(controlCode, listener);
            }
        }

        public virtual void UnsubscribeControl(ControlCode controlCode, UnityAction action)
        {
            if (_listenerLookup.ContainsKey(controlCode))
            {
                _listenerLookup[controlCode].RemoveListener(action);
            }
        }

        public virtual void OnUpdate()
        {
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate()
        {
        }

        public virtual void StopMovement()
        {
            IsUsingJoystick = false;
            JoystickDirection = Vector2.zero;
        }
    }
}