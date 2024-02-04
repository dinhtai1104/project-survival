using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorSetDynamicRigidbodyTask : Game.Tasks.Task
{
    [SerializeField] private Rigidbody2D body;

    public override async UniTask Begin()
    {
        await base.Begin();
        body.bodyType = RigidbodyType2D.Dynamic;

        IsCompleted = true;
    }
}