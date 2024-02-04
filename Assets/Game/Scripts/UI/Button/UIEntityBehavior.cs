using UnityEngine;

public class UIEntityBehaviour : MonoBehaviour
{
    private RectTransform rect;

    public RectTransform RectTransform
    {
        get
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
            return rect;
        }
        set => rect = value;
    }
}