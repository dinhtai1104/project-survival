using System;
using UnityEngine;

namespace Engine
{
    public sealed class NullMovementEngine : IMovementEngine
    {
        public float DirectionSign
        {
            get { return -1f; }
        }

        public Vector3 FacingDirection
        {
            get { return Vector3.right; }
        }

        public Stat Speed
        {
            set { }
            get { return null; }
        }

        public bool LockMovement
        {
            set { }
            get { return true; }
        }


        public bool UsingHorizontalBound
        {
            set { }
            get { return false; }
        }

        public bool Static
        {
            set { }
            get { return true; }
        }

        public Vector3 CurrentDirection
        {
            get { return Vector3.zero; }
        }

        public Vector3 CurrentPosition { get; }

        public bool IsMoving
        {
            set { }
            get { return false; }
        }

        public void Init(Actor actor)
        {
        }

        public void SetBound(Bound2D bound)
        {

        }

        public bool UsingVerticalBound
        {
            get { return false; }

            set { }
        }

        public Bound2D MovementBound => Bound2D.zero;

        public bool ReachBoundLeft => false;

        public bool ReachBoundRight => false;

        public bool ReachBoundTop => false;

        public bool ReachBoundBottom => false;

        public bool ReachBound => false;

        public void Teleport(Vector3 pos)
        {
        }

        public void SyncGraphicRotation(Vector3 dir)
        {
        }

        public void FlipFacingDirection()
        {
        }

        public void SetDirection(Vector3 dir)
        {
        }

        public void LookAt(Vector3 position)
        {
        }

        public bool MoveTo(Vector3 position)
        {
            return false;
        }

        public bool MoveDirection(Vector3 direction)
        {
            return false;
        }

        public void OnUpdate()
        {
        }

        public void CopyState(IMovementEngine movementEngine)
        {
        }

        public bool MoveTo(Vector3 position, float speed)
        {
            return false;
        }

        public bool MoveDirection(Vector3 direction, float speed)
        {
            return false;
        }

        public Vector3 Bound(Vector3 position)
        {
            return Vector3.zero;
        }

        public void Bound()
        {
        }
    }
}