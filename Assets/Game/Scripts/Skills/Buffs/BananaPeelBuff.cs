using Assets.Game.Scripts.Core.BuffHandler;
using Assets.Game.Scripts.Events;
using Core;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Buffs
{
    public class BananaPeelBuff : BuffGameObject
    {
        private float m_Time = 0;
        private float m_TimeStayHeal = 0;
        private float m_HealPercent;

        private bool m_CanUpdate = false;

        protected override void OnInit()
        {
            base.OnInit();
            m_TimeStayHeal = BuffData.GetValue(StatKey.Time, DataGame.Data.EModifierBuff.Skill);
            m_HealPercent = BuffData.GetValue(StatKey.Hp, DataGame.Data.EModifierBuff.Skill);

            Architecture.Get<EventMgr>().Subscribe<WaveBeginEventArgs>(WaveBeginHandler);
            Architecture.Get<EventMgr>().Subscribe<WaveEndEventArgs>(WaveEndHandler);
        }
        protected override void OnExit()
        {
            base.OnExit();

            Architecture.Get<EventMgr>().Subscribe<WaveBeginEventArgs>(WaveBeginHandler);
            Architecture.Get<EventMgr>().Subscribe<WaveEndEventArgs>(WaveEndHandler);
        }

        private void WaveBeginHandler(object sender, IEventArgs e)
        {
            m_CanUpdate = true;
        }

        private void WaveEndHandler(object sender, IEventArgs e)
        {
            m_CanUpdate = false;
        }

        public override void OnUpdate()
        {
            if (!m_CanUpdate) return;
            base.OnUpdate();
            if (Owner.Input.IsUsingJoystick)
            {
                m_Time = 0;
            }
            else
            {
                m_Time += Time.deltaTime;
                if (m_Time > m_TimeStayHeal)
                {
                    m_Time = 0;
                    var hpHeal = Owner.Health.MaxHealth * m_HealPercent;
                    Owner.Health.Healing(hpHeal);
#if DEVELOPMENT
                    Debug.Log($"Healing {hpHeal} by using " + name);
#endif
                }
            }
        }
    }
}
