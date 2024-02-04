using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorFinishAttackTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();

        BulletBase.onBulletDeactive += OnBulletDeactive;
    }
    public override async UniTask End()
    {
        BulletBase.onBulletDeactive += OnBulletDeactive;
        await base.End();
    }
    public override void OnStop()
    {
        base.OnStop();
        BulletBase.onBulletDeactive -= OnBulletDeactive;

    }
    private void OnBulletDeactive(BulletBase bullet, ActorBase actor)
    {
        if (actor == Caster)
        {
            Caster.AnimationHandler.GetAnimator().AnimationState.SetAnimation(1, "attack/combo_1_3", false);
            Caster.AnimationHandler.GetAnimator().AnimationState.AddEmptyAnimation(1, 0, 0);
            IsCompleted = true;
            BulletBase.onBulletDeactive -= OnBulletDeactive;
        }
    }

   
}