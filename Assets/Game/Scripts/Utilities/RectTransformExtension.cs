using UnityEngine;
public static class RectTransformExtension
{
    public static float GetWidth(this RectTransform rt)
    {
        var w = (rt.anchorMax.x - rt.anchorMin.x) * Screen.width + rt.sizeDelta.x;
        return w;
    }

    public static float GetHeight(this RectTransform rt)
    {
        var h = (rt.anchorMax.y - rt.anchorMin.y) * Screen.height + rt.sizeDelta.y;
        return h;
    }
}
