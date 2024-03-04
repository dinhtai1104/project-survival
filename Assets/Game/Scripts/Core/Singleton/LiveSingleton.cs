using UnityEngine;

public class LiveSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}