using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public class Enemy11AnimationHandler : AnimationHandler
    {

        public override void SetAttackIdle()
        {
        }

        public override void SetClimb()
        {
        }

        public override void SetDead()
        {
            ClearTracks();
            anim.AnimationState.SetAnimation(0, "die", false);

        }

        public override void SetGetHit()
        {
            anim.AnimationState.SetAnimation(2, "get_hit", false);
            anim.AnimationState.AddEmptyAnimation(2, 0, 0);

        }

        public override void SetIdle()
        {
            anim.AnimationState.SetAnimation(0, "idle", true);

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
            anim.AnimationState.SetAnimation(0, "move", true);
        }

        public override void SetShoot()
        {
        }

        public override void SetWin()
        {
        }
    }
}