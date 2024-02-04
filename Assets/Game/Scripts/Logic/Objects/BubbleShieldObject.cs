using UnityEngine;

public class BubbleShieldObject : BaseBuffObject
{
    [SerializeField] AutoDestroyObject autoDestroy;
    public float duration = 2f;

    public AutoDestroyObject AutoDestroy => autoDestroy;

    public void SetDuration(float duration)
    {
        this.duration = duration;
        AutoDestroy.SetDuration(duration);
    }
}