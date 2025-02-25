using UnityEngine;

namespace Engine
{
    public interface ICameraController
    {
        Vector3 CameraViewCenterPosition { get; }
        void Shake();

        void AddFollowTarget(Transform target, float duration = 0f);

        void RemoveFollowTarget(Transform target, float duration = 0f);
        void Zoom(float zoomAmount, float duration = 0f);
    }
}
