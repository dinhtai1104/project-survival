using Game.GameActor;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElipseMovementHandler : MonoBehaviour, IMove
{
    [SerializeField, ReadOnly] private float AngleStart;
    [SerializeField, ReadOnly] private float CurrentAngle;
    private IGetPositionByAngle Position;

    public Stat Speed { set; get; }
    public bool IsActive { set; get; }

    public virtual void SetMove(Vector2 move)
    {

    }
    public Vector3 GetDirection()
    {
        return transform.right;
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

    public void TrackTarget(float levelTracking, Transform target)
    {
    }
}
