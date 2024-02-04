using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.UI
{
    [System.Serializable]
    public class CanvasProfile
    {
        [ReadOnly] public string name;
        [ReadOnly] public Vector2 screenSize;
        public float aspectRatio;
    }
}