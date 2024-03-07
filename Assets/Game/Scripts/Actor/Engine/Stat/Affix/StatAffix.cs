using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    [System.Serializable]
    public class StatAffix : BaseAffix
    {
        [SerializeField]
        private string _statName;
        [ShowInInspector]
        private StatModifier _modifier;
        [ShowInInspector]
        private IStatGroup _stats;
        public float Value => _modifier.Value;

        public StatAffix(string statName, StatModifier modifier)
        {
            StatName = statName;
            _modifier = modifier;
            SetDescriptionKey();
        }
        public StatAffix()
        {
        }

        public StatModifier Modifier => _modifier;

        public string StatName { get => _statName; set => _statName = value; }
        public IStatGroup Stats { get => _stats; set => _stats = value; }

        public void ChangeModifier(float newValue)
        {
            Modifier.Value = newValue;
        }

        public override void OnEquip(IStatGroup stats)
        {
            base.OnEquip(stats);
            Stats = stats;
            Stats.AddModifier(StatName, _modifier, this);
        }
        public override void OnUnEquip()
        {
            base.OnUnEquip();
            Stats?.RemoveModifier(StatName, _modifier);
        }

        public override string GetDescription()
        {
            return I2Localize.GetLocalize("Affix/" + DescriptionKey, Modifier.Value);
        }
        public virtual string GetDescriptionParams(params object[] param)
        {
            return I2Localize.GetLocalize("Affix/" + DescriptionKey, param);
        }
        public virtual void SetDescriptionKey()
        {
            DescriptionKey = $"{"Base"}_{StatName}_{Modifier.Type}";
        }

        public override object Clone()
        {
            var clone = (StatAffix)base.Clone();
            return clone;
        }
    }
}