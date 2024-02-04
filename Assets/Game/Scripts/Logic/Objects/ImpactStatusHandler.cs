using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

    public abstract class ImpactStatusHandler:MonoBehaviour
    {
        public abstract UniTask Apply(ActorBase caster, ActorBase target, ImpactHandler handler);
    }
