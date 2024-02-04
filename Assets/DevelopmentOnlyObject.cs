using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentOnlyObject : MonoBehaviour
{
    private void OnEnable()
    {
#if !DEVELOPMENT && !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

}
