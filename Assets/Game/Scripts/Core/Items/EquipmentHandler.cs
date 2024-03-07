using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public class EquipmentHandler 
    {
        private Dictionary<EItemSlot, EquipableItem> m_EquipmentSlots;
        private List<EquipableItem> m_Equipments;
        private IStatGroup m_Stat;
        public event Action<EquipableItem> OnEquipItem;


        public IEnumerable<EquipableItem> EquipedItems => m_Equipments;
        public IStatGroup Stat => m_Stat;

        public EquipmentHandler(IStatGroup stat)
        {
            m_Stat = stat;
            m_EquipmentSlots = new Dictionary<EItemSlot, EquipableItem>();
            m_Equipments = new List<EquipableItem>();

            foreach (var slot in Enum.GetValues(typeof(EItemSlot)) as EItemSlot[])
            {
                m_EquipmentSlots.Add(slot, null);
            }
        }

        public bool IsEquipped(EquipableItem item)
        {
            if (!m_EquipmentSlots.ContainsKey(item.SlotType)) return false;
            return m_EquipmentSlots[item.SlotType] == item;
        }

        public void Equip(EquipableItem item)
        {
            if (item == null)
            {
                Debug.Log("Equip null item");
                return;
            }

            if (m_EquipmentSlots.ContainsKey(item.SlotType))
            {
                var currentItem = m_EquipmentSlots[item.SlotType];
                if (currentItem != null)
                {
                    Unequip(currentItem.SlotType);
                }
            }
            else
            {
                m_EquipmentSlots.Add(item.SlotType, null);
            }

            item.IsEquipped = true;
            m_Equipments.Add(item);
            m_EquipmentSlots[item.SlotType] = item;
            item.OnEquip(m_Stat);
        }

        public void Unequip(EItemSlot slot)
        {
            if (!m_EquipmentSlots.ContainsKey(slot)) return;
            
            var equipItem = m_EquipmentSlots[slot];
            if (equipItem == null) return;

            m_Equipments.Remove(equipItem);

        }
    }
}
