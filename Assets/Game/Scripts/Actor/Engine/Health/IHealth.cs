using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IHealth
    {
        Actor Owner { get; }
        event Action<IHealth> OnValueChanged;
        bool Initialized { set; get; }
        float CurrentHealth { set; get; }
        float MaxHealth { set; get; }
        float MinHealth { set; get; }
        float HealthPercentage { get; }
        bool Invincible { set; get; }
        void Init(Actor actor);
        void Reset();
        void Damage(float damage, EDamageType type);
        void Healing(float amount);
        void Kill();
        void SubscribeReceiveDamageEvent(Action<float, EDamageType> callback);
    }
}
