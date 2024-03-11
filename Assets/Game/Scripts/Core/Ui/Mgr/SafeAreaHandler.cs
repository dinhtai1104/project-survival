using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui.View
{
    public class SafeAreaHandler : MonoBehaviour
    {
        [SerializeField] private Rect rect;
        [SerializeField] private Direction direction = Direction.Vertical;

        // Start is called before the first frame update
        void Awake()
        {
            Safe();
        }

        public void Safe()
        {
            rect = Screen.safeArea;
            RectTransform rt = GetComponent<RectTransform>();
            if (direction == Direction.Vertical)
            {
                rt.offsetMin = new Vector2(rt.offsetMin.x, Screen.height - rect.height - rect.y);
                rt.offsetMax = new Vector2(rt.offsetMax.x, -(Screen.height - rect.height - rect.y));
            }
            else
            {
                rt.offsetMin = new Vector2(rect.x, 0);
                rt.offsetMax = new Vector2(-rect.x, 0);
            }
        }
        private enum Direction
        {
            Horizontal, Vertical
        }
    }
}