using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object _lock = new object();
    protected static T instance;
    private static bool _isQuit = false;
    public static T Instance
    {
        get
        {
            if (!_isQuit)
            {
                lock (_lock)
                {
                    if (!instance) instance = FindObjectOfType<T>();
                    if (!instance)
                    {
                        var type = typeof(T);
                        instance = new GameObject(type.ToString()).AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        instance = this as T;
    }
    protected virtual void OnDestroy()
    {
        instance = null;
    }
    protected virtual void OnApplicationQuit()
    {
        _isQuit = true;
    }
}