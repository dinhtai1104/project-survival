using UnityEngine;

namespace Engine
{
    public static class MovementEngineExtension
    {
        public static bool IsInFrontOf(this IMovementEngine movementEngine, IMovementEngine target)
        {
            var dot = Vector3.Dot(movementEngine.FacingDirection, target.FacingDirection);

            // Attacker is in front of 
            return !Mathf.Approximately(dot, 1f);
        }

        public static bool IsInAngleOf(this IMovementEngine movementEngine, Vector3 center, float angleMin, float angleMax)
        {
            var angle = Vector3.Angle(movementEngine.FacingDirection, center - movementEngine.CurrentPosition);
            return angleMin <= angle && angle <= angleMax;
        }

        public static bool IsBehindOf(this IMovementEngine movementEngine, IMovementEngine target)
        {
            if (movementEngine == null || target == null) return false;

            if (movementEngine.DirectionSign > 0f)
            {
                if (movementEngine.CurrentPosition.x >= target.CurrentPosition.x) return true;
            }
            else
            {
                if (movementEngine.CurrentPosition.x <= target.CurrentPosition.x) return true;
            }

            return false;
        }
    }
}