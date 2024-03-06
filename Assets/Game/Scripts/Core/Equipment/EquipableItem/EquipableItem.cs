using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    [System.Serializable]
    public class EquipableItem
    {
        public string Id;

        // Events
        public event System.Action EnhanceEvent;
        public event System.Action UpRarityEvent;
        public event System.Action EquipEvent;
        public event System.Action UnEquipEvent;

        // Properties
        private int m_ItemLevel;
        private bool m_IsEquipped;
        private EItemSlot m_SlotType;
        private StatAffix m_baseStatAffix;
        private ERarity m_Rarity;
        private List<BaseAffix> m_ListAffixes;

        public EItemSlot SlotType => m_SlotType;
        public StatAffix BaseStatAffix => m_baseStatAffix;
        public ERarity Rarity => m_Rarity;
        public bool IsEquipped { get => m_IsEquipped; set => m_IsEquipped = value; }
        public List<BaseAffix> ListAffixes => m_ListAffixes;


        public EquipableItem() : base()
        {
            m_ListAffixes = new List<BaseAffix>();
        }

        // METHODS

        public void SetBaseStat(StatAffix baseStat)
        {
            BaseStatAffix?.OnRemoveFromItem(this);
            m_baseStatAffix = baseStat;
            baseStat.OnAddToItem(this);
        }

        public void AddBaseAffix(BaseAffix affix)
        {
            ListAffixes.Add(affix);
            affix.OnAddToItem(this);
        }


        public void OnEquip(IStatGroup stats)
        {
            BaseStatAffix.OnEquip(stats);

            foreach (var affix in ListAffixes)
            {
                affix.OnEquip(stats);
            }
            EquipEvent?.Invoke();
        }

        public void OnUnEquip()
        {
            BaseStatAffix.OnUnEquip();
            foreach (var affix in ListAffixes)
            {
                affix.OnUnEquip();
            }
            UnEquipEvent?.Invoke();
        }

        public void Enhance()
        {
            m_ItemLevel++;
            EnhanceEvent?.Invoke();
        }

        public void UpRarity()
        {
            if (Rarity < ERarity.Ultimate)
            {
                // save
                m_Rarity++;
                UpRarityEvent?.Invoke();
            }
        }

        public void SetRarity(ERarity rarity)
        {
            m_Rarity = rarity;
        }

        public EquipableItem DeepCopy()
        {
            var item = (EquipableItem)this.MemberwiseClone();
            item.m_baseStatAffix = (StatAffix)m_baseStatAffix.Clone();
            return item;
        }
    }
}
