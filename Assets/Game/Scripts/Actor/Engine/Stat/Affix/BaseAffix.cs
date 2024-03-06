using Framework;
using System;

namespace Engine
{
    [System.Serializable]
    public abstract class BaseAffix : ICloneable, IComparable<BaseAffix>
    {
        public virtual string DescriptionKey { get; set; } = "Value";
        public virtual string GetDescription()
        {
            return I2Localize.GetLocalize("Affix/" + DescriptionKey);
        }
        public virtual void OnEquip(IStatGroup stats)
        {
        }
        public virtual void OnUnEquip()
        {
        }

        public virtual void OnAddToItem(EquipableItem item)
        {
        }
        public virtual void OnRemoveFromItem(EquipableItem item)
        {
        }

        public virtual int CompareTo(BaseAffix other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var affixDescriptionComparison = string.Compare(DescriptionKey, other.DescriptionKey, StringComparison.Ordinal);
            if (affixDescriptionComparison != 0) return affixDescriptionComparison;
            return 0;
        }


        public virtual object Clone()
        {
            var baseAffix = (BaseAffix)this.MemberwiseClone();
            return baseAffix;
        }
    }
}