using UnityEngine;

public class BulletAccelerateMoveForwardHandler : BulletMoveHandler
{
    [SerializeField]
    private AnimationCurve accelerationRate;
    [SerializeField]
    private float trackingDelay = 0.75f, trackingDuration = 2.5f;
    Vector2 direction ;

    float startTime = 0;
    float trackingTime = 0;
    bool isTracking = false;
    public override void Move(Stat speed, Vector2 move)
    {
        direction = move;
        base.Move(speed, move);
        startTime = Time.time;
    }
    public override void SetDirection(Vector3 direction)
    {
        base.SetDirection(direction);
        this.direction = direction;
    }
    public override void TrackTarget(float trackingRate, Transform target)
    {
        base.TrackTarget(trackingRate, target);
        trackingTime = 0;
        isTracking = trackingRate > 0;
        InvokeRepeating(nameof(UpdateTargetPosition), 0, trackingDelay);

    }
    Vector3 position;
    Vector3 GetTargetPosition()
    {
        return position;
    }
    void UpdateTargetPosition()
    {

        if (target == null)
        {
            CancelInvoke();
            return;
        }

        position = target.position;
        if (Speed == null || Speed.Value == 0 || IsActive == false)
        {
            CancelInvoke();
        }
    }
    public override void OnUpdate()
    {
        float time = (Time.time - startTime) * (unscaleTime ? 1 : GameTime.Controller.TIME_SCALE);
        if (target != null && isTracking && time>=0.6f)
        {

            Vector2 lookAtTargetDirection = (GetTargetPosition() - _transform.position).normalized;

            direction = Vector3.RotateTowards(direction, lookAtTargetDirection, trackingRate * Mathf.PI, 1);

            trackingTime += GameTime.Controller.FixedDeltaTime();
            if (trackingTime > trackingDuration)
            {
                isTracking = false;
            }
        }

        _transform.position += ((Vector3)direction).normalized * 
            (
                Speed.Value * accelerationRate.Evaluate(time)
                * GameTime.Controller.FixedDeltaTime(unscaleTime)
            );
        _transform.right = direction;
    }
}
