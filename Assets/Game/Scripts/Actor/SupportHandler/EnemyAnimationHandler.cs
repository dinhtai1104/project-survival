using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAnimationHandler : AnimationHandler
{
    [SerializeField]
    private  string runAnim = "move", landAnim,jumpAnim,attackAnim= "attack/combo_1",dieAnim="die",getHitAnim="get_hit", idle = "idle";

    public override void SetAttackIdle()
    {
    }

    public override void SetClimb()
    {
    }

    public override void SetDead()
    {
        if (string.IsNullOrEmpty(dieAnim)) return;
        ClearTracks();
        anim.AnimationState.SetAnimation(0, dieAnim, false);

    }

    public override void SetGetHit()
    {
        if (string.IsNullOrEmpty(getHitAnim)) return;
        anim.AnimationState.SetAnimation(2, getHitAnim, false);
        anim.AnimationState.AddEmptyAnimation(2, 0, 0);

    }

    public override void SetIdle()
    {
        anim.AnimationState.SetAnimation(0, idle, true);

    }

    public override void SetJump(int jumpIndex)
    {
        if (string.IsNullOrEmpty(jumpAnim)) return;
        anim.AnimationState.SetAnimation(0, jumpAnim, false);
    }

 
    public override void SetJumpDown()
    {
    }

    public override void SetLand()
    {
        if (string.IsNullOrEmpty(landAnim)) return;
        anim.AnimationState.SetAnimation(0, landAnim, false);
        anim.AnimationState.AddAnimation(0, idle, true, 0);
    }

    public override void SetLandIdle()
    {
        if (string.IsNullOrEmpty(landAnim)) return;
        anim.AnimationState.SetAnimation(0, landAnim, false);
        anim.AnimationState.AddAnimation(0, idle, true, 0);
    }

    public override void SetRun()
    {
        if (anim.AnimationName.Equals(runAnim) || string.IsNullOrEmpty(runAnim)) return;
        anim.AnimationState.SetAnimation(0, runAnim, true);
    }

    public override void SetShoot()
    {
        if (string.IsNullOrEmpty(attackAnim)) return;
        anim.AnimationState.SetAnimation(1, attackAnim, false);

        anim.AnimationState.AddEmptyAnimation(1, 0,0);
    }

    public override void SetWin()
    {
    }
}
