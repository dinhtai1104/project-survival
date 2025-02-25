using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine
{
    [CreateAssetMenu(fileName = "SOStorageActor", menuName = "Storage/StorageActor")]
    public class SOStorageActor : ScriptableObject
    {
        public List<ActorBase> Actors;

        public int Count => Actors.Count;

        public bool Contains(ActorBase actor)
        {
            return Actors.Contains(actor);
        }

        public void Add(ActorBase actor)
        {
            Actors.Add(actor);
        }

        public void Remove(ActorBase actor)
        {
            Actors.Remove(actor);
        }

        public void Clear()
        {
            Actors.Clear();
        }

        public ActorBase GetRandomActor()
        {
            return Actors[Random.Range(0, Actors.Count)];
        }
    }
}
