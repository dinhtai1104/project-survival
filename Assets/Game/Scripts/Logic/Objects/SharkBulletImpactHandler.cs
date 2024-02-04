using Game.GameActor;
using UnityEngine;

public class SharkBulletImpactHandler : ExplodeImpactHandler
{
    private SharkGunWeapon weapon => Base.weaponBase as SharkGunWeapon;
    [SerializeField] private AutoDestroyObject blackHolePrefab;
 
    public override void Impact(ITarget target)
    {
        if ((target != null && (Object)target == this.Base) || (target != null && target.IsDead()))
        {
            return;
        }
        if (target != null && !Base.Caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            return;
        }

        var dmgExplMul = weapon.aoe_DmgExplosionMul.FloatValue;
        var sizeExpl = weapon.aoe_RadiusExplosion.FloatValue;
        var item = weapon.GetEquipableItem();
        if (item.EquipmentRarity >= ERarity.Epic)
        {
            sizeExpl = weapon.epic_aoe_RadiusExplosion.FloatValue;
        }

        SetDmgExplodeMul(dmgExplMul);
        SetExplodeSize(sizeExpl);
        base.Impact(target);

        if (item.EquipmentRarity >= ERarity.Legendary)
        {
            // Spawn BlackHole
            var bl = PoolManager.Instance.Spawn(blackHolePrefab);
            bl.transform.position = transform.position;

            bl.SetDuration(weapon.legendary_aoe_HoleDuration.FloatValue);
        }


        if (Base.destroyOnImpact &&
            //collide with wall but can not bounce
            ((target == null && !Bounce()) ||
            // can not bound and pierce
            (!Bounce() && !Pierce())))
        {
            gameObject.SetActive(false);
        }
    }
    protected bool Pierce()
    {
        Base.currentPiercing++;
        if (Base.currentPiercing < Base.MaxPiercing)
        {
            return true;
        }
        return false;
    }
    Collider2D[] colliders = new Collider2D[3];


    public bool Bounce()
    {
        if (Base.currentBounce < Base.MaxBounce)
        {
            return true;
        }
        return false;
    }

}