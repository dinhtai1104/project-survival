using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour,IDamageDealer
{
    [SerializeField]
    private ParticleSystem startPs, endPs, chargePs;
    [SerializeField]
    private LineRenderer [] lines;
    float burnTime = 1;
    [SerializeField]
    private float range = 5;
    private LayerMask mask;
    WeaponBase weapon;
    [SerializeField]
    private AudioSource audioSource;

    public bool isActive = false;
    public bool isStarted = false;
    int overrideDamage;

    Collider2D excludedCollider;
    Vector2 direction;
    float chargeTimer = 0;
    public void SetUp(WeaponBase weapon,Transform holder,float fireRate,float chargeTime, int overrideDamage = -1)
    {
        foreach (LineRenderer line in lines)
        {
            line.positionCount = 0;
        }
        isStarted = false;
        isActive = true;
        this.overrideDamage = overrideDamage;
        this.weapon = weapon;
        mask = this.weapon.mask;
        this.range = weapon.GetRange();
        burnTime = 1f / Mathf.Min(fireRate,40);
        transform.SetParent(holder,true);
        transform.localPosition = Vector3.zero;
        startPs.gameObject.SetActive(true);
        endPs.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.chargeTimer = chargeTime;
        chargePs.Play();
    }
    public void SetDirection(Vector2 direction)
    {

        this.direction = direction;
    }
    void LaserStart()
    {
        if (isStarted) return;
        isStarted = true;
        
        startPs.Play();
        endPs.Play();
        foreach (LineRenderer line in lines)
        {
            line.positionCount = 2;
        }
        if (Sound.Controller.SfxEnable)
        {
            audioSource.Play();
        }
    }
    private void FixedUpdate()
    {
        if (weapon == null) return;
        if (chargeTimer > 0)
        {
            chargeTimer -= GameTime.Controller.FixedDeltaTime();
            return;
        }
        LaserStart();
        Beam(weapon.character.WeaponHandler.GetAttackPoint(weapon).position, direction);
    }
    ITarget target ;
    public void Beam(Vector3 startPos, Vector3 direction)
    {
        //Debug.DrawLine(startPos, startPos + direction*99,Color.red,0.2f);
        SetLinePosition(0, startPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, range, mask);
        bool impact = false;
        this.target = null;
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider != null && overrideDamage != 0)
            {
                ITarget target = hit.collider.GetComponentInParent<ITarget>();
                if (target == (ITarget)weapon.character || (target!=null &&target.GetCharacterType()==weapon.character.GetCharacterType()) ) continue;
               
                SetLinePosition(1, hit.point);
                endPs.transform.position = hit.point;
                this.target = target;
                impact = true;
                break;

            }
        }
        if (!impact)
        {
            endPs.transform.position = startPos + direction * range;
            SetLinePosition(1, startPos + direction * range);
        }

        
     
    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(lines[0].GetPosition(0), lines[0].GetPosition(1));
    //}
    public ITarget GetTarget()
    {
        return target;
    }

    void SetLinePosition(int index,Vector3 pos)
    {
        foreach(LineRenderer line in lines)
        {
            line.SetPosition(index, pos);
        }
    }
    public void Stop()
    {
        target = null;
        isActive = false;
        endPs.Stop();
        startPs.Stop();
        foreach (LineRenderer line in lines)
        {
            line.positionCount = 0;
        }
        audioSource.Stop();
        gameObject.SetActive(false);
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public Vector3 GetDamagePosition()
    {
        return endPs.transform.position;
    }

}
