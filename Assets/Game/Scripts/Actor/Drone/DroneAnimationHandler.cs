using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAnimationHandler : AnimationHandler
{
    public override void SetAttackIdle()
    {
    }

    public override void SetClimb()
    {
    }

    public override void SetDead()
    {
    }

    public override void SetGetHit()
    {
    }

    public override void SetIdle()
    {
    }

    public override void SetJump(int jumpIndex)
    {
    }

    public override void SetJumpDown()
    {
    }

    public override void SetLand()
    {
    }

    public override void SetLandIdle()
    {
    }

    public override void SetRun()
    {
    }

    public override void SetShoot()
    {
        anim.AnimationState.SetAnimation(0, "combo_1", false);
        anim.AnimationState.AddAnimation(0, "idle", true, 0);
    }

    public override void SetWin()
    {
    }

}
