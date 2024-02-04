using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidOnlyObject : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
#if UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

}
