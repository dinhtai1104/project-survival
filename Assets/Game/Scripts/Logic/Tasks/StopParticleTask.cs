using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class StopParticleTask : Task
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public override async UniTask Begin()
        {
            await base.Begin();
            if (_particleSystem)
            {
                _particleSystem.Stop();
            }
            IsCompleted = true;
        }
    }
}
