using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine
{
    [CreateAssetMenu(fileName = "SOStorageActor", menuName = "Storage/StorageActor")]
    public class SOStorageActor : ScriptableObject
    {
        public List<Actor> Actors;

        public int Count => Actors.Count;

        public bool Contains(Actor actor)
        {
            return Actors.Contains(actor);
        }

        public void Add(Actor actor)
        {
            Actors.Add(actor);
        }

        public void Remove(Actor actor)
        {
            Actors.Remove(actor);
        }

        public void Clear()
        {
            Actors.Clear();
        }

        public Actor GetRandomActor()
        {
            return Actors[Random.Range(0, Actors.Count)];
        }
    }
}
