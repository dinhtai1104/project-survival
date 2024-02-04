using Game.GameActor;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEye : MonoBehaviour
{

    [SerializeField]
    private float BurnRate = 2;

    private Transform _transform;
    private float damage;

    [SerializeField]
    private LineRenderer []lines;

    ActorBase caster;
    bool isActivated = false;
    Vector3 direction;
    float length;
    float hitTime = 0;
    AimLine aimLine;
    [SerializeField]
    private MMF_Player startFb, endFb;
    [SerializeField]
    private ParticleSystem endPS,startPs;
    [SerializeField]
    private AudioClip laserStartSFX, laserLoopSFX, laserEndSFX;

    [SerializeField]
    private ObjectSoundHandler soundHandler;
    public void SetUp(ActorBase caster,float size,float damage, Vector3 direction, float length)
    {
        this.direction = direction;
        this.length = length;
        this.caster = caster;
        _transform = transform;
        this.damage = damage;
        this.laserSize.y = size;
        Game.Pool.GameObjectSpawner.Instance.Get("AimLine", obj => { 
            
            aimLine = obj.GetComponent<AimLine>();
            aimLine.SetUp(_transform, direction, length);
        });
        SetActive(true);
    }
    public void ReadyLaser()
    {
        startFb?.PlayFeedbacks();
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public void HideAimLine()
    {
        aimLine.Hide();
    }
    public void Fire()
    {
        soundHandler.PlayOneShot(laserStartSFX, 1);
        soundHandler.PlayLoop(laserLoopSFX, 1);

        isActivated = true;
        foreach (LineRenderer line in lines)
        {
            line.widthMultiplier = laserSize.y;
            line.positionCount = 2;
            line.SetPosition(0, _transform.position);
            line.SetPosition(1, _transform.position+direction*length);
        }
        laserSize.Set(length,laserSize.y,0);

        startPs.Play();

        endPS.transform.position = _transform.position + direction * length;
        endPS.Play();
    }
    public void Release()
    {
        soundHandler.Stop();
        soundHandler.PlayOneShot(laserEndSFX, 1);
        isActivated = false;
        foreach (LineRenderer line in lines)
        {
            line.positionCount = 0;
        }
        endFb?.PlayFeedbacks();

        startPs.Stop(true);
        endPS.Stop(true);
    }
    Collider2D[] colliders = new Collider2D[5];
    Vector3 laserSize = new Vector3(0,1,0);
    private void Update()
    {
        if (!isActivated) return;
        if (Time.time - hitTime >= 1f / BurnRate)
        {
            int count = Physics2D.OverlapBoxNonAlloc(_transform.position+direction*length/2f, laserSize,0, colliders, caster.AttackHandler.targetMask);
            bool hit = false;
            for(int i=0;i<count;i++)
            {
                if (colliders[i] != null)
                {
                    ITarget target = colliders[i].GetComponentInParent<ITarget>();
                    Impact(target);

                    if (target != null)
                    {
                        hit = true;
                    }
                    colliders[i] = null;

                }
            }
            if (hit)
            {
                hitTime = Time.time;
            }
        }
    }

    public virtual void Impact(ITarget target)
    {
        if ((target != null && (Object)target == this.caster) || (target != null && target.IsDead()))
        {
            return;
        }
        if (target != null && !caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            return;
        }
        if (target != null && damage != 0)
        {

            DamageSource damageSource = new DamageSource(caster, (ActorBase)target, damage,caster);


            var modifier = new ModifierSource(damage);
            damageSource.Value = modifier.Value == 0 ? damage : modifier.Value;


            damageSource.posHit = transform.position;
            damageSource._damageSource = EDamageSource.Weapon;

            target.GetHit(damageSource, caster);
        }

    }
}
