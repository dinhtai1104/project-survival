using Game.GameActor;
using UnityEngine;

public class FlameStatus : BaseStatus, IDamageDealer
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

    private void AttackDamage()
    {
        var dmg = sourceActor.Stats.GetValue(StatKey.Dmg) * dmgMul;
        var dmgSource = new DamageSource
        {
            Attacker = sourceActor,
            Defender = targetActor,
            _damage = new Stat(dmg),
            _damageType = EDamage.Flame,
        };
        dmgSource.posHit = transform.position;
        dmgSource._damageSource = EDamageSource.Effect;

        targetActor.GetHit(dmgSource, this);
    }

    public Transform GetTransform()
    {
        return Target.GetTransform();
    }

    public Vector3 GetDamagePosition()
    {
        return Target.GetMidTransform().position;
    }
}