using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorChargeAtTargetTask : SkillTask
{
    public ValueConfigSearch ChargeSpeedMultiplier,Duration;

    float duration;
    public override async UniTask Begin()
    {
        duration = Duration.FloatValue;
        Caster.Stats.AddModifier(StatKey.SpeedMove, new StatModifier(EStatMod.PercentAdd, 1), this);

        Caster.MoveHandler.Stop();
        Caster.MoveHandler.Move(new Vector2(Caster.Sensor.CurrentTarget==null?0:Caster.Sensor.CurrentTarget.GetPosition().x > Caster.GetPosition().x ? 1 : -1, 0), 1);

        await base.Begin();
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (DetectObstacle(Caster))
            {
                Caster.MoveHandler.Move(new Vector2(-Caster.MoveHandler.lastMove.x, 0), 1);
                ((Character)Caster).SetLookDirection(0, 0);
                ((Character)Caster).SetFacing(Caster.MoveHandler.lastMove.x < 0 ? -1 : 1);
            }
        }
        else
        {
            IsCompleted = true;
        }

    }
    [SerializeField]
    Vector3 offsetLeft = new Vector3(-2, 0), offsetRight = new Vector3(2, 0);
    [SerializeField]
    private float groundCheckOffset = 1;
    [SerializeField]
    private LayerMask wallMask;
    float checkObstacleTimer = 0;
    public bool DetectObstacle(ActorBase actor)
    {
        if (Time.time - checkObstacleTimer < 0.25f) return false;
        checkObstacleTimer = Time.time;
        Vector3 point = ((Character)actor).GetMidTransform().position;
        //detect wall in front of character
        RaycastHit2D wallHit = Physics2D.CircleCast(point + (actor.MoveHandler.move.normalized.x < 0 ? offsetLeft : offsetRight), 0.5f, (actor.MoveHandler.move.normalized.x < 0 ? offsetLeft : offsetRight), 0, wallMask);

        //if there is wall
        if (wallHit.collider != null)
        {
            return true;
        }
        return false;


    }
    public override async UniTask End()
    {
        Caster.Stats.RemoveModifiersFromSource( this);
        Caster.MoveHandler.Stop();
        await base.End();
    }
}
