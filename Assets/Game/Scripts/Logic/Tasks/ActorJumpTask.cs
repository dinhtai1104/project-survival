using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorJumpTask : SkillTask
{
    [UnityEngine.SerializeField]
    private Vector2 jumpForce;
    public override async UniTask Begin()
    {
        var dir = Caster.MoveHandler.lastMove;
        var rch = Physics2D.Raycast(Caster.GetMidPos(), dir, 2f, LayerMask.NameToLayer("Ground"));
        if (rch.collider)
        {
            dir.x *= -1;
        }
        Caster.SetFacing(dir.x);

        if (jumpForce.magnitude != 0)
        {
            Caster.MoveHandler.Jump(jumpForce.normalized, jumpForce.magnitude);
        }
        else
        {
            Caster.MoveHandler.Jump();

        }

        await base.Begin();
        IsCompleted = true;
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return; 
        if (!Caster.MoveHandler.isGrounded &&Caster.GetRigidbody().velocity.y<=0)
        {
            Caster.GetRigidbody().velocity = UnityEngine.Vector2.zero;
            IsCompleted = true;
        }
    }
}
