using UnityEngine;
using UnityEngine.VFX;

public interface IMove
{
    Stat Speed { set; get; }
    bool IsActive { get; set; }
    void Move();
    void SetMove(Vector2 move);
    void Move(Stat speed, UnityEngine.Vector2 direction);
    void TrackTarget(float levelTracking, Transform target);
    Vector3 GetDirection( );
    void SetDirection(Vector3 direction);
    void SetPosition(Vector3 position);
    void Stop();
    void OnUpdate();
}
