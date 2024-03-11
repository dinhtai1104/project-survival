using Assets.Game.Scripts.Actor.States.Common;
using Engine;
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
        private List<(UIButtonSkill, BaseSkillState)> m_MappingSkillState = new List<(UIButtonSkill, BaseSkillState)>();
        private Engine.Actor m_MainActor;
        [SerializeField] private Joystick m_Joystick;
        [SerializeField] private UIButtonSkill m_DashSkillBtn;

        private void OnEnable()
        {
            m_Joystick.onMoveStart.AddListener(OnMovingStart);
            m_Joystick.onMove.AddListener(OnMoving);
            m_Joystick.onMoveEnd.AddListener(OnMovingEnd);
            m_DashSkillBtn.SetCallback(OnDash);
        }
        private void OnDash()
        {
            Messenger.Broadcast(EventKey.InputDash);
        }
        private void OnDisable()
        {
            m_Joystick.OnPointerUp(null);
            m_Joystick.onMoveStart.RemoveListener(OnMovingStart);
            m_Joystick.onMove.RemoveListener(OnMoving);
            m_Joystick.onMoveEnd.RemoveListener(OnMovingEnd);
        }

        public void Setup(Engine.Actor actor)
        {
            this.m_MainActor = actor;
            MappingState<ActorDashState>(m_DashSkillBtn);
        }

        private void OnMovingStart()
        {
            Messenger.Broadcast(EventKey.InputJoystickStart);
        }

        private void OnMoving(Vector2 input)
        {
            Messenger.Broadcast(EventKey.InputJoystickMovement, input);
        }

        private void OnMovingEnd()
        {
            Messenger.Broadcast(EventKey.InputJoystickEnd);
        }

        public void OnExecute()
        {
            foreach (var (buttonView, state) in m_MappingSkillState)
            {
                if (!state.IsCooldowning)
                {
                    buttonView.SetInteractable(true);
                }
                else
                {
                    buttonView.SetInteractable(false);
                    buttonView.SetCooldownText(state.CooldownTimer);
                    buttonView.SetCooldownProgress(state.RemaningCooldownProgress);
                }
            }
        }
        private void MappingState<T>(UIButtonSkill skillBtn) where T : BaseSkillState
        {
            if (m_MainActor.Fsm.HasState<T>())
            {
                var state = m_MainActor.Fsm.GetState<T>();
                m_MappingSkillState.Add((skillBtn, state));
            }
            else
            {
                skillBtn.Empty = true;
            }
        }
    }
}
