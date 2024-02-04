using UnityEngine;

public class HitActionSpawnExplode : MonoBehaviour, IHitTriggerAction
{
    public DamageExplode dmgExplodePrefab;

    public ValueConfigSearch explode_Radius;
    public ValueConfigSearch explode_Dmg;
    public void Action(Collider2D collider)
    {
        var Caster = GetComponent<CharacterObjectBase>().Caster;
        var explode = PoolManager.Instance.Spawn(dmgExplodePrefab);
        explode.transform.position = transform.position;
        explode.Init(Caster);
        explode.SetSize(explode_Radius.FloatValue);
        explode.SetDmg(Caster.Stats.GetValue(StatKey.Dmg) * explode_Dmg.FloatValue);
        explode.Explode();
    }

    public void Action()
    {
        var Caster = GetComponent<CharacterObjectBase>().Caster;
        var explode = PoolManager.Instance.Spawn(dmgExplodePrefab);
        explode.transform.position = transform.position;
        explode.Init(Caster);
        explode.SetSize(explode_Radius.FloatValue);
        explode.SetDmg(Caster.Stats.GetValue(StatKey.Dmg) * explode_Dmg.FloatValue);
        explode.Explode();
    }
}
