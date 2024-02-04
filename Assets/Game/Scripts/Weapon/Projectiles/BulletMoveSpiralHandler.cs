using UnityEngine;

public class BulletMoveSpiralHandler : BulletMoveHandler
{
   
    
    public float RotateOffset=1,Direction=1;
    Vector2 startPoint;
    float rotation = 0;
    [HideInInspector]
    public int targetRotation = 0;
    [HideInInspector]
    public float rotateSpeed = 0;

    public override void Move(Stat speed, Vector2 move)
    {
        Messenger.Broadcast<BulletMoveSpiralHandler>(EventKey.OnBulletMove, this);
        startPoint = transform.position;
        rotation = 0;
        base.Move(speed, move);
    }
    private void OnDisable()
    {
    }
    public override void OnUpdate()
    {
        if (Vector3.Distance(_transform.position, startPoint) < RotateOffset)
        {
            Vector3 direction = move;
            _transform.position += (direction.normalized * Speed.Value) * GameTime.Controller.FixedDeltaTime(unscaleTime);
        }
        else
        {
            if (rotation <= targetRotation)
            {
                float rot = rotateSpeed*Direction * GameTime.Controller.FixedDeltaTime(unscaleTime);
                _transform.RotateAround(startPoint, Vector3.forward, rot);

                rotation += Mathf.Abs(rot);
            }
            else
            {
                Vector3 direction = move;
                _transform.position += (direction.normalized * Speed.Value) * GameTime.Controller.FixedDeltaTime(unscaleTime);
            }
        }
    }
}