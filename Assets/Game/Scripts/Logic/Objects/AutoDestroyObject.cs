using UnityEngine;
using UnityEngine.Events;

public class AutoDestroyObject : MonoBehaviour
{
    [SerializeField] private float timeDestroy = 3f;
    [SerializeField] protected UnityEvent DestroyEvent;
    [SerializeField] private bool isAutoDestroy = false;

    protected virtual float TimeDestroy => timeDestroy;
    public OnComplete onComplete;
    private bool isDestroying = false;
    private float time = 0;
    public void SetDuration(float duration)
    {
        onComplete = null;
        timeDestroy = duration;
        time = 0;
        isDestroying = true;
    }
    protected virtual void OnEnable()
    {
        time = 0;
    }
    private void OnDisable()
    {
        isDestroying = false;
    }
    private void Update()
    {
        if (!isAutoDestroy)
        {
            if (!isDestroying) return;
        }
        time += Time.deltaTime;

        if (time > TimeDestroy)
        {
            onComplete?.Invoke();
            DestroyEvent?.Invoke();
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}