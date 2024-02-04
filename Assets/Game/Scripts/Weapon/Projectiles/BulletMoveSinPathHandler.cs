using UnityEngine;

public class BulletMoveSinPathHandler : BulletMoveHandler
{
    public ValueConfigSearch Amplitude;
    public ValueConfigSearch Frequency;
    public AnimationCurve AmplitudeCurve;

    public float amplitude ;
    public float frequency ;

    Vector3 foward;
    float a = 0;

    public override void Move()
    {
        base.Move();
        foward = _transform.position;
        a = 0;
        amplitude = this.Amplitude.FloatValue;
        frequency = this.Frequency.FloatValue;
    }
    public override void Move(Stat speed, Vector2 move)
    {
        base.Move(speed, move);
        foward = _transform.position;
        a = 0;
        amplitude = this.Amplitude.FloatValue;
        frequency = this.Frequency.FloatValue;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
       
        Vector3 velocity = move * Speed.Value;
        Vector3 offset = _transform.up * (Mathf.Sin(a*Mathf.PI*2*frequency) * amplitude*AmplitudeCurve.Evaluate(a));
        a += GameTime.Controller.FixedDeltaTime(unscaleTime);

        foward += (velocity) * GameTime.Controller.FixedDeltaTime(unscaleTime);
        _transform.position =foward +offset;
    }
}
