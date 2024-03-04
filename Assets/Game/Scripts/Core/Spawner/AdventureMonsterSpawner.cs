using com.mec;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class AdventureMonsterSpawner : BaseMonsterSpawner
    {
        private List<Actor> m_ActorSpawned;
        public AdventureMonsterSpawner(MonsterFactory monsterFactory, Bound2D spawnBound) : base(monsterFactory, spawnBound)
        {
            m_ActorSpawned = new List<Actor>();
            Messenger.AddListener<Actor>(EventKey.ActorDie, OnActorDie);
        }

        private void OnActorDie(Actor actor)
        {
            if (!m_ActorSpawned.Contains(actor))
            {
                return;
            }

            m_ActorSpawned.Remove(actor);
            if (actor.Tagger.HasTag(Tags.BossTag))
            {
                // Boss die => all enemies die
                foreach (var spawned in m_ActorSpawned)
                {
                    spawned.Health.Invincible = true;
                    spawned.Health.CurrentHealth = 0;
                }
            }
        }

        protected override void AddToSpawnedActor(Actor actor)
        {
            m_ActorSpawned.Add(actor);
        }

        protected override IEnumerator<float> _Spawn(float delaySpawn)
        {
            yield return Timing.WaitForSeconds(delaySpawn);

        }
    }
}
