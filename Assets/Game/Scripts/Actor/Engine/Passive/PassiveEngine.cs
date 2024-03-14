using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine
{
    public class PassiveEngine : MonoBehaviour, IPassiveEngine
    {
        private Actor m_Owner;
        private List<IPassive> m_Passives;

        public void Init(Actor owner)
        {
            m_Passives = new List<IPassive>();
            this.m_Owner = owner;
        }

        public void AddPassive(IPassive passive)
        {
            if (m_Passives.Contains(passive)) return;
            m_Passives.Add(passive);
            passive.Owner = m_Owner;
            passive.Equip();
        }

        public void OnUpdate()
        {
            for (int i = 0; i < m_Passives.Count; i++)
            {
                var passive = m_Passives[i];
                if (passive == null) continue;
                passive.OnUpdate();
            }
        }

        public void RemovePassive(IPassive passive)
        {
            if (!m_Passives.Contains(passive)) return;
            passive.UnEquip();
            m_Passives.Remove(passive);
        }
    }
}
