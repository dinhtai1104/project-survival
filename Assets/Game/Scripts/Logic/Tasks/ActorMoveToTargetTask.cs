using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorMoveToTargetTask : SkillTask
{
    Vector3 destination;
    public ValueConfigSearch Speed;
    public float offset = 0;
    public LayerMask obstacleLayer;
    float startTime = 0;
    public override async UniTask Begin()
    {
        await base.Begin();

        if (Caster.Sensor.CurrentTarget == null)
        {
            IsCompleted = true;
            return;
        }
        startTime = Time.time;
         var direction = (Caster.Sensor.CurrentTarget.GetMidTransform().position - Caster.GetPosition());
        //var hit = Physics2D.Raycast(Caster.GetMidTransform().position, direction, 999, layerMask: obstacleLayer);
        //Debug.DrawRay(Caster.GetMidTransform().position, direction*hit.distance, Color.red, 1);
        //float maxLength = 99;
        //if (hit.collider != null)
        //{
        //    maxLength = hit.distance-2;
        //}
        //float distance = Vector3.Distance(Caster.GetMidTransform().position, Caster.Sensor.CurrentTarget.GetMidTransform().position);
        //destination = Caster.GetMidTransform().position+direction*Mathf.Clamp(distance+offset,1,maxLength);
        destination = Caster.GetPosition() + direction ;
        Debug.DrawRay(Caster.GetMidTransform().position, direction , Color.red, 1);

        Caster.MoveHandler.SpeedMultiply = Speed.SetId(Caster.gameObject.name).FloatValue;
    }


    public override UniTask End()
    {
        return base.End();
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if ( (Time.time-startTime<0.4f||Caster.GetPosition().y<10 )&&Vector3.Distance(Caster.GetPosition(),destination)>1f)
        {
            var direction = (destination - Caster.GetPosition()).normalized;
            Caster.MoveHandler.Move(direction, 1);
        }
        else
        {
            Caster.MoveHandler.SpeedMultiply = 1;

            IsCompleted = true;
            return;
        }

        var hit = Physics2D.CircleCast(Caster.GetMidPos(), Caster.transform.localScale.x * 0.5f, (destination - Caster.GetPosition()).normalized, 1f, obstacleLayer);
        if (hit.collider != null)
        {
            Caster.MoveHandler.SpeedMultiply = 1;

            IsCompleted = true;
            return;
        }
    }
}
