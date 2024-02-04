using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

public class UITaskSetTriggerAnimator : Task
{
    public Animator animator;
    public string trigger;

    public override async UniTask Begin()
    {
        await base.Begin();
        animator.SetTrigger(trigger);
        IsCompleted = true;
    }
}

