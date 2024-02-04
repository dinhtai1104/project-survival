using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : AnimationHandler
{
    private const string BLINK = "blink";

    public override void SetUp(Character character)
    {
        base.SetUp(character);
        return;
    }
    private void OnEnable()
    {
        Invoke(nameof(Blink),2);

    }
    public override void OnDisable()
    {
        CancelInvoke();
    }
    public override void OnDestroy()
    {
        CancelInvoke();

    }

    void Blink()
    {
        if (character==null||character.IsDead()) return;
        try
        {
            anim.AnimationState.SetAnimation(5, BLINK, false);
            anim.AnimationState.AddEmptyAnimation(5, 0, 0);
            Invoke(nameof(Blink), UnityEngine.Random.Range(0.5f, 5f));
        }
        catch (System.Exception e)
        {

        }
    }
    public override void SetWin()
    {
    }

    public override void SetRun()
    {
        
        var track=anim.AnimationState.SetAnimation(0, $"run/{(character.AttackHandler.canTrigger ? "fight" : "normal")}", true);
    }

    public override void SetJump(int jumpIndex)
    {
        switch (jumpIndex)
        {
            case 0:
                anim.AnimationState.SetAnimation(0, $"jump/up", true);
                break;
            default:
                anim.AnimationState.SetAnimation(0, $"jump/double", false).MixDuration=0;
                anim.AnimationState.AddAnimation(0, $"jump/up", true,0).MixDuration=0;
                break;
        }
    }

    public override void SetJumpDown()
    {
        anim.AnimationState.SetAnimation(0, $"jump/down", true);
    }
    public override void SetLand()
    {
        //anim.AnimationState.SetAnimation(0, $"jump/grounding", false);
        //anim.AnimationState.AddEmptyAnimation(1, 0, 0);
        anim.AnimationState.SetAnimation(0, $"run/{(character.AttackHandler.canTrigger ? "fight" : "normal")}", true);
    }

    public override void SetLandIdle()
    {
        ClearTracks();
        anim.AnimationState.SetAnimation(0, $"jump/grounding", false);
        if (character.MoveHandler.isClimbing)
        {
            anim.AnimationState.AddAnimation(0, $"climb/grounding", true, 0);
            anim.AnimationState.AddAnimation(0, $"idle/{(character.AttackHandler.canTrigger ? "fight" : "normal")}", true, 0);

        }
        else
        {
            anim.AnimationState.AddAnimation(0, $"idle/{(character.AttackHandler.canTrigger ? "fight" : "normal")}", true, 0);
        }
    }
    public override void SetClimb()
    {
        ClearTracks();
        anim.AnimationState.SetAnimation(0, $"climb/start", true);
    }

    public override void SetDead()
    {
        ClearTracks();
        anim.AnimationState.SetAnimation(0, $"die/1", false);

    }

    public override void SetGetHit()
    {
        anim.AnimationState.SetAnimation(2, $"get_hit/1", false);
        anim.AnimationState.AddEmptyAnimation(2, 0, 0);

    }

    public override void SetIdle()
    {
        //ClearTracks();
        anim.AnimationState.SetAnimation(0, $"idle/{(character.AttackHandler.canTrigger ? "fight" : "normal")}", true);
    }

    public override void SetShoot()
    {
        anim.AnimationState.SetAnimation(1, $"attack/normal", false);
        anim.AnimationState.AddEmptyAnimation(1, 0, 0);

    }

    public override void SetAttackIdle()
    {
    }
}
