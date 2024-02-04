using UnityEngine;

public class UICheat : MonoBehaviour
{
    protected void Start()
    {
#if DEVELOPMENT || UNITY_EDITOR
        Init();
#else
        gameObject.SetActive(false);
#endif
    }
    protected virtual void Init()
    {

    }
}