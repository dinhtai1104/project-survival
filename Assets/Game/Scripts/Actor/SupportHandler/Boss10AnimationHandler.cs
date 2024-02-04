using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class Boss10AnimationHandler : BossAnimationHandler
{

    [Header("Animation")]
    [SerializeField] private string startJump;
    [SerializeField] private string loopJumping;
    [SerializeField] private string highestJumping;
    [SerializeField] private string Falling;
    [SerializeField] private string fallGrounded;


    public override void SetJump(int jumpIndex)
    {
        base.SetJump(jumpIndex);
        string animTarget;
        bool isLoop = false;
        switch (jumpIndex)
        {
            case 0:
                animTarget = startJump;
                break;
            case 1:
                animTarget = loopJumping;
                isLoop = true;
                break;
            case 2:
                animTarget = highestJumping;
                isLoop = true;
                break;
            case 3:
                animTarget = Falling;
                isLoop = true;
                break;
            case 4:
                animTarget = fallGrounded;
                break;
            default:
                animTarget = startJump;
                break;
        }
        anim.AnimationState.SetAnimation(0, animTarget, isLoop);
    }
}