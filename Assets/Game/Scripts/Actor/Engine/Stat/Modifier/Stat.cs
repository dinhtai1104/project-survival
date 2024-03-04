using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{

    /// <summary>
    /// Stat is object contains stat value and modifier attached to this stat, we use StatModifier to control value Stat
    /// </summary>
    [Serializable]
    public class Stat
    {
        [SerializeField] private float _baseValue;
        [SerializeField] private float _minValue = 0f;
        [SerializeField] private float _maxValue = float.MaxValue;
        private float _lastValue;
        [ShowInInspector]
        private float _value;

        [SerializeField] private List<StatModifier> attributeModifiers;
        public float BaseValue { get => _baseValue; set => _baseValue = value; }
        public float LastValue { get => _lastValue; set => _lastValue = value; }
        public float Value { get => _value; set => _value = value; }

        public float ConstraintMin => _minValue;
        public float ConstraintMax => _maxValue;

        public List<StatModifier> AttributeModifiers => attributeModifiers;

        private readonly List<Action<float>> _listeners;


        public Stat()
        {
            attributeModifiers = new List<StatModifier>();
            _listeners = new List<Action<float>>();
        }
        public Stat(float baseValue) : this()
        {
            BaseValue = baseValue;
            Value = BaseValue;
        }

        public Stat(float baseValue, float minValue) : this(baseValue, minValue, Mathf.Infinity)
        {
        }
        public Stat(float baseValue, float min, float max) : this(baseValue)
        {
            SetConstraintMin(min);
            SetConstraintMax(max);
        }

        // Add listener when detect any change stat
        public void AddListener(Action<float> callback)
        {
            if (!_listeners.Contains(callback))
            {
                _listeners.Add(callback);
                callback(Value);
            }
        }

        public void RemoveListener(Action<float> callback)
        {
            if (_listeners.Contains(callback))
            {
                _listeners.Remove(callback);
            }
        }

        public void InvokeListeners()
        {
            foreach (var listener in _listeners)
            {
                listener?.Invoke(_value);
            }
        }

        public void ClearListeners()
        {
            _listeners.Clear();
        }
        public void SetConstraintMin(float min)
        {
            if (min > _maxValue)
            {
                Debug.LogError("Min value " + min + " cannot be greater than Max value  " + _maxValue);
                return;
            }

            _minValue = min;
        }

        public void SetConstraintMax(float max)
        {
            if (max < _minValue)
            {
                Debug.LogError("Max value " + max + " cannot be smaller than Min value  " + _minValue);
                return;
            }

            _maxValue = max;
        }
        public virtual StatModifier GetModifier(int index)
        {
            return AttributeModifiers[index];
        }
        public virtual void AddModifier(StatModifier mod)
        {
#if UNITY_EDITOR
            if (AttributeModifiers.Contains(mod))
                Debug.LogWarning("Duplicate mod: " + mod);
#endif

            mod.RecalculateValueAction = RecalculateValue;
            AttributeModifiers.Add(mod);
            RecalculateValue();
            InvokeListeners();
        }
        public virtual void RemoveModifier(StatModifier mod)
        {
            if (AttributeModifiers.Contains(mod))
            {
                AttributeModifiers.Remove(mod);
                RecalculateValue();
                InvokeListeners();
            }
        }

        public void RecalculateValue()
        {
            LastValue = Value;
            float baseValue = _baseValue;
            Value = AttributeModifiers != null ? CalculateFinalValuePOE(baseValue) : baseValue;
            Value = Mathf.Clamp(Value, _minValue, _maxValue);

            if (Math.Abs(Value - LastValue) > 1e-4)
            {
                LastValue = Value;
            }
        }

        /// <summary>
        /// Follow formula: Value = Sum(Flat) x (1 + Sum(Increase) - Sum(Reduce)) x Sum(1 + More) x Sum(1 - Less)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual float CalculateFinalValuePOE(float baseValue)
        {
            var finalValue = baseValue;
            var sumPercentAdd = 0f;
            var sumPercentMulMore = 0f;
            var sumPercentMulLess = 0f;
            var flatMul = 0f;
            var percent = 1f;

            for (int i = 0; i < AttributeModifiers.Count; i++)
            {
                StatModifier mod = AttributeModifiers[i];
                switch (mod.Type)
                {
                    case EStatMod.Flat:
                        finalValue += mod.Value;
                        break;
                    case EStatMod.PercentAdd:
                        {
                            sumPercentAdd += mod.Value;
                            break;
                        }

                    case EStatMod.PercentMul:

                        // More
                        if (mod.Value >= 0f)
                        {
                            sumPercentMulMore += mod.Value;
                        }
                        // Less
                        else
                        {
                            sumPercentMulLess += mod.Value;
                        }

                        break;
                    case EStatMod.FlatMul:
                        flatMul += mod.Value;
                        break;
                    case EStatMod.Percent:
                        percent *= mod.Value;
                        break;
                }
            }

            // Percent Add (Increase, Decrease)
            finalValue *= 1f + sumPercentAdd;

            // Percent Mul (More, Less)
            finalValue *= 1f + sumPercentMulMore;
            finalValue *= 1f + sumPercentMulLess;
            finalValue *= percent;

            if (flatMul > 0)
            {
                finalValue *= flatMul;
            }
            return finalValue;
        }

        public bool HasModifier(StatModifier modifier)
        {
            return AttributeModifiers.Contains(modifier);
        }
        public void RemoveAllModifiersFromSource(object source)
        {
            AttributeModifiers.RemoveAll(t => t.Source == source);
            RecalculateValue();
        }

        public void ClearModifiers()
        {
            AttributeModifiers.Clear();
            RecalculateValue();
        }
    }
}