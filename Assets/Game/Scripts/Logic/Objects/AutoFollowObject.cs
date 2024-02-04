using com.mec;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoFollowObject : MonoBehaviour
{
    public Transform target;
    public void SetFollow(Transform follow)
    {
        target = follow;
    }
    private void Update()
    {
        if (target == null) return;
        transform.position = target.position;
    }
}