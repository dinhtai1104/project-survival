using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputController { 
    public class InputController : MonoBehaviour
    {
        public static bool ENABLED = true;
        public MovePad movePad;
        public ButtonController[] buttons;

        private void OnEnable()
        {
            Messenger.AddListener(EventKey.EnableControl, OnControlEnabled);
            Messenger.AddListener(EventKey.DisableControl, OnControlDisabled);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener(EventKey.EnableControl, OnControlEnabled);
            Messenger.RemoveListener(EventKey.DisableControl, OnControlDisabled);
        }
        void OnControlEnabled()
        {
            ENABLED = true;
        }
        void OnControlDisabled()
        {
            ENABLED = false;
        }
        public void Register(System.Action<Vector2,float> onMove,System.Action onStop,params ControlAction[] onButtonTriggered)
        {
            movePad.Register(
                onMove: onMove
             ,
                onStop:onStop
            );
            for(int i = 0; i < onButtonTriggered.Length; i++)
            {
                RegisterControlAction(i,onButtonTriggered[i]);
            }
        }
        public void Deregister(System.Action<Vector2, float> onMove, System.Action onStop, params ControlAction[] onButtonTriggered)
        {
            movePad.Deregister(onMove, onStop);
            for (int i = 0; i < onButtonTriggered.Length; i++)
            {
                DeregisterControlAction(i, onButtonTriggered[i]);
            }
        }
        public ButtonController GetButton(int index)
        {
            return buttons[index];
        }
        public void RegisterControlAction(int index,ControlAction action)
        {
            if (action == null) return;
            buttons[index].Register(action);
        }
        public void DeregisterControlAction(int index, ControlAction action)
        {
            if (action == null) return;
            buttons[index].Deregister(action);
        }
        public void Destroy()
        { 

        }
        public void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}

