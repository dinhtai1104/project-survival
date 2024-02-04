using UnityEngine;

public class NullMoveBullet : IMove
{
    public Stat Speed { set; get; } = new Stat();

    public bool IsActive { set; get; }

    public virtual void SetMove(Vector2 move)
    {

    }
    public void Move()
    {
    }

    public void Move(Stat speed, Vector2 direction)
    {
    }

    public void OnUpdate()
    {
    }

    public void SetDirection(Vector3 direction)
    {
    }

    public void SetPosition(Vector3 position)
    {
    }

    public void Stop()
    {
    }
    public Vector3 GetDirection()
    {
        return Vector3.zero;
    }
    public void TrackTarget(float levelTracking, Transform target)
    {
    }
}