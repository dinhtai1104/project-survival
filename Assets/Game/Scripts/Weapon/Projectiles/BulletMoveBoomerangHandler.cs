using UnityEngine;

public class BulletMoveBoomerangHandler : BulletMoveHandler
{
    protected override void FixedUpdate()
    {
    }
    public override void OnUpdate()
    {
        if (_transform == null || Speed==null) return;
        _transform.position += (move * Speed.Value) * (Time.fixedDeltaTime*(unscaleTime ? 1 : GameTime.Controller.TIME_SCALE));
    }
}
