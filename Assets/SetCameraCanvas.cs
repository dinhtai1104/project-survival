using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraCanvas : MonoBehaviour
{
    void Start()
    {
        //get ui camera
        GetComponent<Canvas>().worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
    }

}
