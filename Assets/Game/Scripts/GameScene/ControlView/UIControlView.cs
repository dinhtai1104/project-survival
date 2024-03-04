using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.ControlView
{
    public class UIControlView : MonoBehaviour
    {
        [SerializeField] private Joystick m_Joystick;

        private void OnEnable()
        {
            m_Joystick.onMoveStart.AddListener(OnMovingStart);
            m_Joystick.onMove.AddListener(OnMoving);
            m_Joystick.onMoveEnd.AddListener(OnMovingEnd);
        }
        private void OnDisable()
        {
            m_Joystick.onMoveStart.RemoveListener(OnMovingStart);
            m_Joystick.onMove.RemoveListener(OnMoving);
            m_Joystick.onMoveEnd.RemoveListener(OnMovingEnd);
        }

        private void OnMovingStart()
        {
            Messenger.Broadcast(EventKey.InputJoystickStart);
        }

        private void OnMoving(Vector2 input)
        {
            Messenger.Broadcast(EventKey.InputJoystickMovement, input);
            Logger.Log(input);
        }

        private void OnMovingEnd()
        {
            Messenger.Broadcast(EventKey.InputJoystickEnd);
        }
    }
}
