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
    public class HolyGrailBuff : BuffGameObject
    {
        private StatModifier m_SpeedModifier;
        private StatModifier m_DamageBonusModifier;
        protected override void OnInit()
        {
            base.OnInit();
            m_SpeedModifier = new StatModifier(EStatMod.PercentAdd, BuffData.GetValue(StatKey.Speed, DataGame.Data.EModifierBuff.Skill));
            m_DamageBonusModifier = new StatModifier(EStatMod.Flat, BuffData.GetValue(StatKey.DamageBonus, DataGame.Data.EModifierBuff.Skill));
            GameArchitecture.GetService<IEventMgrService>().Subscribe<WaveBeginEventArgs>(WaveBeginEventHandler);
        }

        private void WaveBeginEventHandler(object sender, IEventArgs e)
        {
#if DEVELOPMENT
            var lastDamageBonus = Owner.Stats.GetValue(StatKey.DamageBonus);
            var lastSpeed = Owner.Stats.GetValue(StatKey.Speed);
#endif
            Owner.Stats.AddModifier(StatKey.DamageBonus, m_DamageBonusModifier, this);
            Owner.Stats.AddModifier(StatKey.Speed, m_SpeedModifier, this);

#if DEVELOPMENT
            var nowDamageBonus = Owner.Stats.GetValue(StatKey.DamageBonus);
            var nowSpeed = Owner.Stats.GetValue(StatKey.Speed);
#endif
#if DEVELOPMENT
            Debug.Log($"Change DamageBonus | From {lastDamageBonus} to {nowDamageBonus} ==> Modifier Value: {m_DamageBonusModifier.ToString()} | using by " + name, gameObject);
            Debug.Log($"Change Speed  Move | From {lastSpeed} to {nowSpeed} ==> Modifier Value: {m_SpeedModifier.ToString()} | using by " + name, gameObject);
#endif
        }

        protected override void OnExit()
        {
            base.OnExit();
            Owner.Stats.RemoveModifiersFromSource(this);
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<WaveBeginEventArgs>(WaveBeginEventHandler);
        }
    }
}
