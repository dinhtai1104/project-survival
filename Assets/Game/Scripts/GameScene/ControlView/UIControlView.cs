using Assets.Game.Scripts.Events;
using Core;
using Engine;
using Engine.State.Common;
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

        private IEventArgs m_JoystickStartMoveEvent;
        private IEventArgs m_JoystickMoveEvent;
        private IEventArgs m_JoystickEndMoveEvent;
        private IEventArgs m_DashEvent;

        private void Awake()
        {
            m_DashEvent = new InputButtonSkillEventArgs();
            (m_DashEvent as InputButtonSkillEventArgs).ControlCode = EControlCode.Dash;
            m_JoystickStartMoveEvent = new JoystickMovementStartEventArgs();
            m_JoystickMoveEvent = new JoystickMovementEventArgs();
            m_JoystickEndMoveEvent = new JoystickMovementEndEventArgs();
        }


        private void OnEnable()
        {
            m_Joystick.onMoveStart.AddListener(OnMovingStart);
            m_Joystick.onMove.AddListener(OnMoving);
            m_Joystick.onMoveEnd.AddListener(OnMovingEnd);
            m_DashSkillBtn.SetCallback(OnDash);
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

        private void OnDash()
        {
            Architecture.Get<EventMgr>().FireNow(this, m_DashEvent);
        }

        private void OnMovingStart()
        {
            Architecture.Get<EventMgr>().FireNow(this, m_JoystickStartMoveEvent);
        }

        private void OnMoving(Vector2 input)
        {
            (m_JoystickMoveEvent as JoystickMovementEventArgs).m_Direction = input;
            Architecture.Get<EventMgr>().FireNow(this, m_JoystickMoveEvent);
        }

        private void OnMovingEnd()
        {
            Architecture.Get<EventMgr>().FireNow(this, m_JoystickEndMoveEvent);
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
