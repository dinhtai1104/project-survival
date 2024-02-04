using UnityEngine;

public class PositionHell : MonoBehaviour
{

    private void Start()
    {
#if UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }
}