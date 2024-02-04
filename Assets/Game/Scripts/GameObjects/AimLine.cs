using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    LineRenderer line2;
    Transform triggerPoint;
    Vector3 direction;
    float length;
    public void SetUp(Transform triggerPoint,Vector2 direction,float length)
    {
        this.length = length;
        this.triggerPoint = triggerPoint;
        this.direction = direction;
        line.positionCount = 2;
        gameObject.SetActive(true);
        SetLine();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (triggerPoint == null) return;
        SetLine();
    }
    void SetLine()
    {
        line.SetPosition(0, triggerPoint.position);

        line.SetPosition(1, triggerPoint.position + direction * length);

        line2.SetPosition(0, triggerPoint.position);

        line2.SetPosition(1, triggerPoint.position + direction * length);
    }
}
