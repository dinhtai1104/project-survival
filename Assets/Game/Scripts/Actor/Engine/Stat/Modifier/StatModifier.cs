using Sirenix.OdinInspector;
using System;

namespace Engine
{

    /// <summary>
    /// StatModifier to modifier any value
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        public EStatMod Type;
        [ShowInInspector]
        private float value;
        public object Source;
        [NonSerialized]
        private Action recalculateValueAction;

        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                RecalculateValueAction?.Invoke();
            }
        }

        public Action RecalculateValueAction { get => recalculateValueAction; set => recalculateValueAction = value; }

        public StatModifier() { }
        public StatModifier(EStatMod mod, float value)
        {
            Type = mod;
            this.value = value;
        }

        public StatModifier(EStatMod type, float value, object source)
        {
            Type = type;
            this.value = value;
            Source = source;
        }


        public string ToString(string format)
        {
            //return string.Format($"+ {Value}:{format}");
            return "+ " + Value.ToString(format);
        }

        public override string ToString()
        {
            var pre = value > 0 ? "+" : "-";
            var aff = "";

            switch (Type)
            {
                case EStatMod.Flat:
                    break;
                case EStatMod.PercentAdd:
                    aff = "%";
                    break;
                case EStatMod.PercentMul:
                    aff = "%";
                    break;
                case EStatMod.FlatMul:
                    aff = "";
                    break;
                case EStatMod.Percent:
                    aff = "%";
                    break;
            }

            return pre + Value + aff;
        }

        public StatModifier Clone()
        {
            return new StatModifier(Type, value);
        }
    }
}