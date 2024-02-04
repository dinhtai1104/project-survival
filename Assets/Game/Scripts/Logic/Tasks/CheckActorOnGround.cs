public class CheckActorOnGround : SkillTask
{
    public override void Run()
    {
        base.Run();
        if (Caster.MoveHandler.isGrounded) IsCompleted = true;
    }
    public override void OnStop()
    {
        base.OnStop();
        IsCompleted = true;
    }
}