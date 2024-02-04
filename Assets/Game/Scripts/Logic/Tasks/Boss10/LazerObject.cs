using System;
using UnityEngine;
public delegate void OnComplete();

[RequireComponent(typeof(AutoDestroyObject))]
[RequireComponent(typeof(InvokeHitByTrigger))]

public class LazerObject : BulletSimpleDamageObject
{
    Transform _transform;
    private Stat _rotateSpeedStat;
    [SerializeField] private AutoDestroyObject _autoDestroyObject;
    [SerializeField] private Transform endPoint;

    public void Play(float angleStart, Stat _dmg, float timeDestroy)
    {
        _transform = transform;
        DmgStat = _dmg;
        transform.eulerAngles = new UnityEngine.Vector3(0, 0, angleStart);

        _autoDestroyObject.SetDuration(timeDestroy);

        _hit.SetIsFullTimeHit(true);
        base.Play();
        SetMaxHitToTarget(999999);
    }
    public void SetDirection(Vector2 direction)
    {
        _transform.right = direction;
    }

    public override Vector3 GetDamagePosition()
    {
        return endPoint.position;
    }
    public void SetOnDestroyCallback(OnComplete complete)
    {
        _autoDestroyObject.onComplete += complete;
    }
}
