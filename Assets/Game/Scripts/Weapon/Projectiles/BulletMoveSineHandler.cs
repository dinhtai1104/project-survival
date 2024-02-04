using UnityEngine;

public class BulletMoveSineHandler : BulletMoveHandler
{
    public ValueConfigSearch amplitude;
    public ValueConfigSearch frequency;

    public override void OnUpdate()
    {
        base.OnUpdate();
        float amplitude = this.amplitude.FloatValue;
        float frequency = this.frequency.FloatValue;
        Vector3 velocity = move * Speed.Value;
        Vector3 offset = transform.up * Mathf.Cos(Time.time * frequency) * amplitude;
        _transform.position += (offset + velocity) * (Time.fixedDeltaTime* (unscaleTime?1:(GameTime.Controller.TIME_SCALE)));
    }
}
