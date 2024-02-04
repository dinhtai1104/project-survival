using Game.GameActor;
using UnityEngine;

public class BleedStatus : BaseStatus, IDamageDealer
{
    private float timeHandler = 0;
    private float dmgMul;
    private float cooldown;
    public override void SetDmgMul(float dmgMul)
    {
        this.dmgMul = dmgMul;
    }
    public override void SetCooldown(float cooldown)
    {
        this.cooldown = cooldown;
    }
    public override void OnUpdate(float deltaTime)
    {
        timeHandler += deltaTime;
        // replace this to cooldownPoison of Source Character
        float cooldownPoison = cooldown;
        if (timeHandler > cooldownPoison)
        {
            timeHandler = 0;
            AttackDamage();
        }
    }

    protected virtual void AttackDamage()
    {
        if (targetActor.IsDead())
        {
            return;
        }
        var dmg = sourceActor.Stats.GetValue(StatKey.Dmg) * dmgMul;
        var dmgSource = new DamageSource(sourceActor, targetActor, dmg,this);
        dmgSource._damageType = EDamage.Bleeding;
        dmgSource.posHit = transform.position;
        dmgSource._damageSource = EDamageSource.Effect;
        targetActor.GetHit(dmgSource, this);
    }

    public Transform GetTransform()
    {
        return Target.transform;
    }

    public Vector3 GetDamagePosition()
    {
        if (Target == null) return Vector3.zero;
        return Target.GetDamagePosition();
    }
}