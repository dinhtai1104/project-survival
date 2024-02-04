using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class HitActionShotByPattern : MonoBehaviour, IHitTriggerAction
{
    public PatternBulletHellBase patternPrefab;
    public BulletSimpleDamageObject bulletPrefab;

    public ValueConfigSearch miniBullet_Size;
    public ValueConfigSearch miniBullet_Velocity;
    public ValueConfigSearch miniBullet_Dmg;
    public void Action(Collider2D collider)
    {
        var bullet = GetComponent<CharacterObjectBase>();
        var Caster = bullet.Caster;

        var pt = PoolManager.Instance.Spawn(patternPrefab);
        pt.transform.position = transform.position;


        var lastDir = bullet.transform.right;
        var pos = bullet.transform.position - lastDir;
        Vector3 rotation = Vector3.zero;
        // Wall check
        if (Physics2D.Raycast(pos, Vector3.down, 1))
        {
            rotation = Vector3.zero;
        }
        else if (Physics2D.Raycast(pos, Vector3.left, 1))
        {
            rotation = new Vector3(0, 0, -90);
        }
        else
        {
            rotation = new Vector3(0, 0, 90);
        }
        pt.transform.eulerAngles = rotation;

        pt.Prepare(bulletPrefab, Caster);
        pt.SetMaxHit(1);
        pt.SetDmgBullet(new Stat(Caster.Stats.GetValue(StatKey.Dmg) * miniBullet_Dmg.FloatValue));

        var statSpeed = new Stat(miniBullet_Velocity.FloatValue);
        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        pt.SetSpeed(statSpeed);
        pt.SetSizeBullet(miniBullet_Size.FloatValue);

        pt.Play();
    }
}
