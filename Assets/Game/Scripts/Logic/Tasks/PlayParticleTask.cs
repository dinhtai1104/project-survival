using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

public class PlayParticleTask : Task
{
    [SerializeField] private bool isUseTime = false;
    [SerializeField] private ValueConfigSearch timeBloomStage1;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private bool _isCompleteWhenParticleFinish = false;
    public override async UniTask Begin()
    {
        if (isUseTime)
        {
            foreach (var ps in _particleSystem.GetComponentsInChildren<ParticleSystem>())
            {
                var main = ps.main;
                main.startLifetime = new ParticleSystem.MinMaxCurve(timeBloomStage1.FloatValue, timeBloomStage1.FloatValue);
            }
        }

        if (_particleSystem)
        {
            _particleSystem.Play();
        }
   
        await base.Begin();

        if (isUseTime)
            await UniTask.Delay((int)(timeBloomStage1.FloatValue * 1000));
        IsCompleted = true;
    }
}