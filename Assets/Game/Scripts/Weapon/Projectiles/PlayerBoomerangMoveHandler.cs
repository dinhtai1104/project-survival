using com.mec;
using Game.GameActor;
using System;
using UnityEngine;

public class PlayerBoomerangMoveHandler : MonoBehaviour, IMove
{
    public BulletBase bulletBase;
    public Stat Speed { set; get; }

    public bool IsActive { set; get; }
    private Vector3 direction = Vector3.zero;
    private CoroutineHandle moveHandler;

    private void OnEnable()
    {
        bulletBase.onTrigger += OnTrigger;
    }

    private void OnTrigger(Collider2D collider, ITarget target)
    {
    }

    public virtual void SetMove(Vector2 move)
    {

    }
    public void Move()
    {
        SetDirection(transform.right);
    }
    private void OnDisable()
    {
        bulletBase.onTrigger -= OnTrigger;
        Timing.KillCoroutines(moveHandler);
    }

    public void Move(Stat speed, Vector2 direction)
    {
        SetDirection(direction);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public Vector3 GetDirection()
    {
        return direction;
    }
    public void TrackTarget(float levelTracking, Transform target)
    {
    }

    public void OnUpdate()
    {
    }

    public void SetPosition(Vector3 position)
    {
    }
    public void Stop()
    {
    }
}