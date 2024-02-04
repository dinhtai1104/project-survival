using Game.GameActor;
using UnityEngine;

public class SpawnExplodeAction : MonoBehaviour, IStateEnterCallback
{
    public DamageExplode dmgExplodePrefab;

    public ValueConfigSearch explode_Radius;
    public ValueConfigSearch explode_Dmg;

    public ActorBase caster;

    public void Action()
    {
        if (caster == null)
        {
            if (GetComponent<CharacterObjectBase>())
            {
                caster = GetComponent<CharacterObjectBase>().Caster;
            }
        }
        if (caster == null) return;
        var explode = PoolManager.Instance.Spawn(dmgExplodePrefab);
        explode.transform.position = transform.position;
        explode.Init(caster);
        explode.SetSize(explode_Radius.FloatValue);
        explode.SetDmg(caster.Stats.GetValue(StatKey.Dmg) * explode_Dmg.FloatValue);
        explode.Explode();
    }

    public void SetActor(ActorBase actor)
    {
        caster = actor;
    }

}