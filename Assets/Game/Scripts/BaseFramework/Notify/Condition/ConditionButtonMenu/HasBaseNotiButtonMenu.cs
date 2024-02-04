using UnityEngine;
public class HasBaseNotiButtonMenu : NotifyCondition
{
    protected bool hasNoti = false;
    protected float time = 0;
    public override bool Validate()
    {
        return hasNoti;
    }

#if DEVELOPMENT
    private void Update()
    {
        if (time > 5f)
        {
            time = 0;
            hasNoti = Random.Range(0, 2) == 1;
        }
        time += Time.deltaTime;
    }
#endif
}
