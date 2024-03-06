using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ActorStatBridge : IDisposable
    {
        private readonly Actor m_Actor;

        public ActorStatBridge(Actor actor)
        {
            m_Actor = actor;
            IStatGroup stats = m_Actor.Stats;

            stats.AddListener(StatKey.Hp, OnUpdateHealth);

            m_Actor.Health.CurrentHealth = m_Actor.Health.MaxHealth = m_Actor.Stats.GetValue(StatKey.Hp);
            m_Actor.Movement.Speed = m_Actor.Stats.GetStat(StatKey.Speed);
            m_Actor.Health.Initialized = true;
        }

        private void OnUpdateHealth(float value)
        {
            m_Actor.Health.MaxHealth = value;
        }

        public void Reset()
        {
            m_Actor.Health.Reset();
            m_Actor.Health.MaxHealth = m_Actor.Stats.GetValue(StatKey.Hp);
            m_Actor.Health.CurrentHealth = m_Actor.Health.MaxHealth;
            m_Actor.Movement.Speed = m_Actor.Stats.GetStat(StatKey.Speed);
            m_Actor.Health.Initialized = true;
        }
        public void Dispose()
        {
            IStatGroup stats = m_Actor.Stats;
            stats.RemoveListener(StatKey.Hp, OnUpdateHealth);
        }
    }
}
