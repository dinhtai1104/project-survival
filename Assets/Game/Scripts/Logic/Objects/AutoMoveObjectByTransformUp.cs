using System.Collections.Generic;
using UnityEngine;

public class AutoMoveObjectByTransformUp : AutoMoveObjectDirect
{
    protected override IEnumerator<float> _Moving()
    {
        direction = transform.up;
        while (true)
        {
            transform.localPosition += direction * Speed.Value * Time.deltaTime;
            yield return Time.deltaTime;
        }
    }
}