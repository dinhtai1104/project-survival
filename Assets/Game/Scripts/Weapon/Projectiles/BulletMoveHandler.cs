using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveHandler : MonoBehaviour,IMove
{
    public Stat Speed { get; set; }

    public bool unscaleTime = false;
    public bool IsActive { get => gameObject.activeSelf; set { } }


    protected bool isMoving = false;
    public Vector3 move;
    public Vector3 lastTrackDirection;
    protected Transform _transform;
    protected Rigidbody2D rb;
    protected float trackingRate;
    protected Transform target;
    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = transform;
    }

    public virtual void SetMove(Vector2 move)
    {
        this.move = move;
    }

    public virtual void Move(Stat speed,Vector2 move)
    {
        isMoving = true;
        this.move = move;
        this.Speed = speed;
       
    }
    public virtual void TrackTarget(float trackingRate, Transform target)
    {
        this.trackingRate = trackingRate;
        this.target = target;
    }
    private void OnDisable()
    {
        Speed?.ClearModifiers();
        isMoving = false;
    }
    protected virtual void FixedUpdate()
    {
        if (isMoving)
        {
            OnUpdate();
        }
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void Move()
    {
        isMoving = true;
    }

    public virtual void SetDirection(Vector3 direction)
    {
        this.move = direction;
    }
    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void Stop()
    {
        isMoving = false;
    }

    public Vector3 GetDirection()
    {
        return move;
    }
}
