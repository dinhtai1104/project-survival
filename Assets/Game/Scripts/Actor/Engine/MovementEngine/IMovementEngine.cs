using UnityEngine;

namespace Engine
{
    public interface IMovementEngine
    {
        float DirectionSign { get; }
        Vector3 FacingDirection { get; }
        Stat Speed { set; get; }
        bool LockMovement { set; get; }
        bool UsingHorizontalBound { set; get; }
        bool UsingVerticalBound { set; get; }
        bool IsMoving { set; get; }
        bool Static { set; get; }
        Bound2D MovementBound { get; }
        bool ReachBoundLeft { get; }
        bool ReachBoundRight { get; }
        bool ReachBoundTop { get; }
        bool ReachBoundBottom { get; }
        bool ReachBound { get; }

        Vector3 CurrentDirection { get; }
        Vector3 CurrentPosition { get; }

        void Init(Actor actor);

        void SetBound(Bound2D bound);

        void Teleport(Vector3 pos);

        void SyncGraphicRotation(Vector3 dir);
        void SetDirection(Vector3 dir);

        void FlipFacingDirection();

        void LookAt(Vector3 position);

        bool MoveTo(Vector3 position);

        bool MoveDirection(Vector3 direction);

        void OnUpdate();

        void CopyState(IMovementEngine movementEngine);

        bool MoveTo(Vector3 position, float speedMul);

        bool MoveDirection(Vector3 direction, float speedMul);

        Vector3 Bound(Vector3 position);

        void Bound();
    }
}
