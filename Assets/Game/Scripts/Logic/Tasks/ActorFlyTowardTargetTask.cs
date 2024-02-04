using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorFlyTowardTargetTask : SkillTask
{
    public ValueConfigSearch SpeedMultiplier,Threshold,Acceleration;
    public float TimeOut = 1.5f;
    public AnimationCurve curve;
    private float threshold, acceleration, time = 0, maxTime = 0;
    public float MaxHeight=10;
    private Vector3 destination;
    public override async UniTask Begin()
    {
        if (Caster.Sensor.CurrentTarget != null)
        {
            time = 0;

            isStop = false;

            Caster.Stats.AddModifier(StatKey.SpeedMove, new StatModifier(EStatMod.PercentMul, SpeedMultiplier.FloatValue), this);
            destination = Caster.Sensor.CurrentTarget.GetPosition() + (Vector3)UnityEngine.Random.insideUnitCircle;
            destination.y = Mathf.Min(11, destination.y);
            threshold = UnityEngine.Random.Range(Threshold.FloatValue - 1f, Threshold.FloatValue + 1f);
            acceleration = Acceleration.FloatValue;
            maxTime = curve.keys[curve.length - 1].time;


            Caster.MoveHandler.Stop();
            Caster.MoveHandler.Move((destination - Caster.GetPosition()).normalized, 1);
            startTime = Time.time;
            await base.Begin();
        }
        else
        {
            await base.Begin();
            IsCompleted = true;
        }
    }
    bool isStop = false;
    float startTime = 0;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (!isStop)
        {
            time += GameTime.Controller.DeltaTime();

            float distance = Vector3.Distance(Caster.GetPosition(), destination);
            if (distance < threshold ||time>=TimeOut ||  (Time.time-startTime>0.4f &&Caster.GetPosition().y>=MaxHeight))
            {
                isStop = true;
                time = 0;
            }
        }
        else
        {
            Caster.MoveHandler.SpeedMultiply = curve.Evaluate(time);
            time += GameTime.Controller.DeltaTime() ;
            if (time >= maxTime || Caster.GetPosition().y >= MaxHeight)
            {
                IsCompleted = true;
            }
        }
    }
    public override async UniTask End()
    {
        Caster.Stats.RemoveModifiersFromSource(this);
        Caster.MoveHandler.Stop();

        Caster.MoveHandler.SpeedMultiply = 1;
        await base.End();
    }

    float CalculateTime(float a, float b, float c)
    {
        float delta = b * b - 4 * a * c;
        if (delta < 0)
        {
            return 0;
        }
        else if (delta == 0)
        {
            return -b / (2 * a);
        }
        else
        {
            delta = Mathf.Sqrt(delta);
            float x1 = (-b + delta) / (2 * a);
            float x2 = (-b - delta) / (2 * a);
            return x1>0?x1:x2;
        }
    }
}
