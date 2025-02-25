using Assets.Game.Scripts.Core.BuffHandler;
using Assets.Game.Scripts.DataGame.Data;
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
    public class AlarmClockBuff : BuffGameObject
    {
        private float m_Time = 0;
        private StatModifier m_DamageBonusModifier;
        private float m_TimeEachBuff;
        private float m_ValueModifier;
        private float m_MaxValue;
        private bool m_CanUpdate = false;
        protected override void OnInit()
        {
            base.OnInit();
            m_DamageBonusModifier = new StatModifier(EStatMod.Flat, 0);
            Owner.Stats.AddModifier(StatKey.DamageBonus, m_DamageBonusModifier, this);

            m_TimeEachBuff = BuffData.GetValue(StatKey.Time, EModifierBuff.Skill);
            m_ValueModifier = BuffData.GetValue(StatKey.DamageBonus, EModifierBuff.Skill);
            m_MaxValue = BuffData.GetValue(StatKey.Max, EModifierBuff.Skill);

            GameArchitecture.GetService<IEventMgrService>().Subscribe<WaveBeginEventArgs>(WaveBeginEventHandler);
            GameArchitecture.GetService<IEventMgrService>().Subscribe<WaveEndEventArgs>(WaveEndEventHandler);
        }

        protected override void OnExit()
        {
            base.OnExit();
            Owner.Stats.RemoveModifier(StatKey.DamageBonus, m_DamageBonusModifier);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<WaveBeginEventArgs>(WaveBeginEventHandler);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<WaveEndEventArgs>(WaveEndEventHandler);
        }

        private void WaveBeginEventHandler(object sender, IEventArgs e)
        {
            m_DamageBonusModifier.Value = 0;
            m_Time = 0;
            m_CanUpdate = true;
        }

        private void WaveEndEventHandler(object sender, IEventArgs e)
        {
            m_Time = 0;
            m_CanUpdate = false;
            m_DamageBonusModifier.Value = 0;
        }

        public override void OnUpdate()
        {
            if (m_CanUpdate == false) return;
            base.OnUpdate();
            m_Time += Time.deltaTime;

            if (m_Time > m_TimeEachBuff)
            {
                m_Time = 0;
                if (m_DamageBonusModifier.Value < m_MaxValue - m_ValueModifier)
                {
                    m_DamageBonusModifier.Value += m_ValueModifier;
#if DEVELOPMENT
                    Debug.Log($"Add {m_ValueModifier} %DamageBonus using by " + name);
#endif
                }
            }
        }
    }
}
