using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPatternBulletOnDespawn : MonoBehaviour, IBeforeDestroyObject
{
    public PatternBulletHellBase patternPrefab;
    public BulletSimpleDamageObject bulletPrefab;
    private CharacterObjectBase objectBase;

    public ValueConfigSearch miniBullet_Dmg;
    public ValueConfigSearch miniBullet_Size;
    public ValueConfigSearch miniBullet_Velocity;
    public ValueConfigSearch miniBullet_Reflect;
    private void Awake()
    {
        objectBase = GetComponent<CharacterObjectBase>();
    }
    public void Action(Collider2D collision)
    {
        var Caster = objectBase.Caster;

        var pt = PoolManager.Instance.Spawn(patternPrefab);


        var pos = objectBase.transform.position - objectBase.transform.right;


        var hit = Physics2D.Raycast(pos, objectBase.transform.right, 3, LayerMask.GetMask("Ground","HitBox"));
        pt.transform.up = hit.normal;
        //pt.transform.position = hit.point+hit.normal*1.5f;
        pt.transform.position = pos;
        //Debug.DrawLine(pos, pos + objectBase.transform.right * hit.distance, Color.red, 2);
        //Debug.DrawLine(pt.transform.position, pt.transform.position +(Vector3)hit.normal * 1, Color.green, 2);

        pt.Prepare(bulletPrefab, Caster);
        pt.SetMaxHit(1);
        pt.SetDmgBullet(new Stat(Caster.Stats.GetValue(StatKey.Dmg) * miniBullet_Dmg.FloatValue));

        var statSpeed = new Stat(miniBullet_Velocity.FloatValue);
        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        
        pt.SetSpeed(statSpeed);
        pt.SetSizeBullet(miniBullet_Size.FloatValue);
        pt.SetMaxHit(miniBullet_Reflect.IntValue);

        pt.Play();
    }
}
