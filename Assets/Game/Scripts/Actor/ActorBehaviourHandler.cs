using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public class ActorBehaviourHandler : MonoBehaviour, IActorBehaviourHandler
    {
        ActorBase Actor;
        [SerializeField]
        private CharacterBehaviour[] behaviours;
        [SerializeField]
        private List<CharacterBehaviour> behaviourInstances = new List<CharacterBehaviour>();
        public void SetUp(ActorBase actor)
        {
            this.Actor = actor;
            behaviourInstances.Clear();
            foreach (var behaviour in behaviours)
            {
                behaviourInstances.Add(behaviour.SetUp(Actor));
            }
            Actor.onActorDie += OnActorDie;
            Actor.onGetHit += OnGetHit;
        }
        public void Destroy()
        {
            for (int i = 0; i < behaviourInstances.Count; i++)
            {
                Destroy(behaviourInstances[i]);
            }
            behaviourInstances.Clear();
            Actor.onActorDie -= OnActorDie;
            Actor.onGetHit -= OnGetHit;
        }

        private void OnGetHit(DamageSource damageSource, IDamageDealer damageDealer)
        {
            foreach (var behaviour in behaviourInstances)
            {
                behaviour.OnGetHit(Actor);
            }
        }

        private void OnActorDie()
        {
            foreach (var behaviour in behaviourInstances)
            {
                behaviour.OnDead(Actor);
                behaviour.OnDeactive(Actor);
            }
        }


        private void Update()
        {
            foreach (var behaviour in behaviourInstances)
            {
                behaviour.OnUpdate(Actor);
            }
        }
        public void StartBehaviours()
        {
            foreach (var behaviour in behaviourInstances)
            {
                behaviour.OnActive(Actor);
            }
        }
        public void StopBehaviours()
        {
            foreach (var behaviour in behaviourInstances)
            {
                behaviour.OnDeactive(Actor);
            }
        }
    }
}