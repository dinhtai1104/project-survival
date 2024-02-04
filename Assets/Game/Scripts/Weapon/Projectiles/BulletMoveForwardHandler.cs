using UnityEngine;

public class BulletMoveForwardHandler : BulletMoveHandler
{
    bool isTracking = false;
    [SerializeField]
    private float trackingDelay = 0.75f, trackingDuration = 2.5f;
    float trackingTime = 0;
    Vector2 direction ;

    public override void Move(Stat speed, Vector2 move)
    {
        direction = move;
        base.Move(speed, move);

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
        isTracking = trackingRate>0;
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
        if (target != null && isTracking )
        {
          
            Vector2 lookAtTargetDirection = (GetTargetPosition() - _transform.position).normalized;

            direction = Vector3.RotateTowards(direction, lookAtTargetDirection, trackingRate * Mathf.PI, 1);


            trackingTime += GameTime.Controller.FixedDeltaTime();
            if (trackingTime > trackingDuration)
            {
                isTracking = false;
            }
        }
      

        _transform.position += ((Vector3)direction.normalized * Speed.Value)* GameTime.Controller.FixedDeltaTime(unscaleTime);
        _transform.right = direction;

    }
}
