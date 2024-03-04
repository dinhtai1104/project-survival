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
            var pre = "";
            var aff = "";

            switch (Type)
            {
                case EStatMod.Flat:
                    pre = "+";
                    break;
                case EStatMod.PercentAdd:
                    pre = "+";
                    aff = "%";
                    break;
                case EStatMod.PercentMul:
                    pre = "x";
                    aff = "%";
                    break;
                case EStatMod.FlatMul:
                    pre = "x";
                    aff = "";
                    break;
                case EStatMod.Percent:
                    pre = "+";
                    aff = "%";
                    break;
            }

            return pre + " " + Value + aff;
        }
    }
}