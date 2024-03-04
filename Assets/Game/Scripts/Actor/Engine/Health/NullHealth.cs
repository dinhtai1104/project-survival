using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullHealth : IHealth
    {
        public Actor Owner { get; private set; }
        public event Action<IHealth> OnValueChanged;
        public bool Initialized { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float MinHealth { get; set; }

        public float HealthPercentage
        {
            get { return 0f; }
        }

        public bool Invincible { get; set; }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public void Reset()
        {
        }

        public void Damage(float damage, EDamageType type)
        {
        }

        public void Healing(float amount)
        {
        }

        public void Kill()
        {
        }

        public void SubscribeReceiveDamageEvent(Action<float, EDamageType> callback)
        {
        }
    }
}