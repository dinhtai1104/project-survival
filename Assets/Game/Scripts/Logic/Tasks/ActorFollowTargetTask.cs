using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorFollowTargetTask : SkillTask
{
    public ValueConfigSearch Speed;
    public ValueConfigSearch Duration;
    public float minDistance = 0;
    public float trackDelay = 0.2f;

    float duration;
    public LayerMask obstacleLayer;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.MoveHandler.SpeedMultiply = Speed.SetId(Caster.gameObject.name).FloatValue;
        duration =Duration.SetId(Caster.gameObject.name).FloatValue;
        time = 0;
        trackTime = 0;
    }


    public override UniTask End()
    {
        return base.End();
    }
    float time = 0,trackTime=0;
    public Vector2 direction;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (time < duration && Caster.Sensor.CurrentTarget!=null && Mathf.Abs(Caster.GetPosition().x-Caster.Sensor.CurrentTarget.GetPosition().x)>minDistance)
        {
            time += GameTime.Controller.DeltaTime();
            if (trackTime < trackDelay)
            {
                trackTime += GameTime.Controller.DeltaTime();
            }
            else
            {
                trackTime = 0;
                direction.x = Caster.Sensor.CurrentTarget.GetPosition().x > Caster.GetPosition().x ? 1 : -1;
                Caster.MoveHandler.Move(direction, 1);
            }
        }
        else
        {
            Caster.MoveHandler.SpeedMultiply =1;
            IsCompleted = true;
        }
    }
}
