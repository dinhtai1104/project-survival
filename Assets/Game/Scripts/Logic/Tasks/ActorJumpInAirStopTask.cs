using Cysharp.Threading.Tasks;
using Spine;
using System;
using UnityEngine;

public class ActorJumpInAirStopTask : SkillTask
{
    public float jumpForce;
    public float timeJump = 0.3f;
    private float timeCtr = 0;
    private bool IsJump = false;
    public string nextAnim = "";

    public override async UniTask Begin()
    {
        IsJump = false;
        await base.Begin();
        timeCtr = 0;
        Caster.AnimationHandler.SetJump(0);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        var animation = trackEntry.Animation.Name;
        var eventKey = e.Data.Name;
        if (eventKey == "jump_tracking")
        {
            IsJump = true;
            Caster.MoveHandler.Jump(Vector3.up, jumpForce);
            //Caster.AnimationHandler.SetAnimation(nextAnim, true);
        }
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        return base.End();
    }
    public override void Run()
    {
        if (!IsJump) return;
        base.Run();
        if (IsCompleted || !IsRunning) return;
        timeCtr += Time.deltaTime;
        if (timeCtr >= timeJump)
        {
            Caster.MoveHandler.move = Vector3.zero;
            Caster.GetRigidbody().bodyType = UnityEngine.RigidbodyType2D.Kinematic;
            Caster.GetRigidbody().velocity = UnityEngine.Vector2.zero;
            //Caster.MoveHandler.Stop();
            Caster.MoveHandler.ClearBoostVelocity();
            IsCompleted = true;
        }
    }
    public override void OnStop()
    {
        base.OnStop();
        Caster.GetRigidbody().bodyType = UnityEngine.RigidbodyType2D.Dynamic;
    }
}