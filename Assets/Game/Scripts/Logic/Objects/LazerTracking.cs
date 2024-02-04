using System;
using UnityEngine;

public class LazerTracking : MonoBehaviour
{
    public float delay = 0.5f;
    private Transform target;
    private Stat rotateSpeed;

    private float time = 0;

    public void SetStartPos(Vector3 pos)
    {
        var direction = pos - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = targetRotation;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    private void OnEnable()
    {
        time = 0;
    }

    public void SetRotation(Stat rotateSpeed)
    {
        this.rotateSpeed = rotateSpeed;
    }

    private void Update()
    {
        if (time < delay)
        {
            time += Time.deltaTime;
            return;
        }
        var direction = target.position - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed.Value * Time.deltaTime);
    }
}