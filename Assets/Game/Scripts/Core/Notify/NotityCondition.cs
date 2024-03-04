using UnityEngine;

public abstract class NotifyCondition : MonoBehaviour
{
    protected NotifiableMono notifyMono;
    protected virtual void Awake()
    {
        notifyMono = GetComponentInChildren<NotifiableMono>(true);
    }
    public abstract bool Validate();
}

public abstract class NotifyCondition<T> : NotifyCondition
{
    public abstract bool Validate(T dependency);
}