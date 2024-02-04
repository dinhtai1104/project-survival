using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PatternBulletHellBase : MonoBehaviour
{
    public List<PositionHell> hellPos = new List<PositionHell>();
    private List<BulletSimpleDamageObject> _bullets;
    private void OnValidate()
    {
        hellPos = new List<PositionHell>(GetComponentsInChildren<PositionHell>(true));
    }
    private void OnEnable()
    {
        
    }
    public void Prepare(BulletSimpleDamageObject prefab, ActorBase Caster)
    {
        hellPos = new List<PositionHell>(GetComponentsInChildren<PositionHell>(true));

        _bullets = new List<BulletSimpleDamageObject>();
        foreach (var hell in hellPos)
        {
            var bullet = PoolManager.Instance.Spawn(prefab, transform);
            bullet.SetCaster(Caster);
            bullet.transform.localPosition = hell.transform.localPosition;
            bullet.transform.localRotation = hell.transform.localRotation;
            bullet.transform.SetParent(null);
            _bullets.Add(bullet);
        }
    }

    public void SetSizeBullet(float size)
    {
        foreach (var bullet in _bullets)
        {
            bullet.transform.localScale = Vector3.one * size;
        }
    }
    public void SetDmgBullet(Stat dmgBullet)
    {
        foreach (var bullet in _bullets)
        {
            bullet.DmgStat = dmgBullet;
        }
    }
    public void SetMaxHit(int maxHit)
    {
        foreach (var bullet in _bullets)
        {
            bullet.SetMaxHit(maxHit);
        }
    }

    public void Play()
    {
        foreach (var bullet in _bullets)
        {
            bullet.Play();
        }
        _bullets.Clear();
        PoolManager.Instance.Despawn(gameObject);
    }

    public void SetSpeed(Stat speed)
    {
        foreach (var bullet in _bullets)
        {
            bullet.Movement.Speed = speed;
        }
    }

#if DEVELOPMENT
    [Range(0, 10)]
    public float radiusDebug;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusDebug);
    }
#endif
}