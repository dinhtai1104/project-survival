using com.mec;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RifleGunPassive : WeaponPassive
{
    public ValueConfigSearch epic_ShootNumberCounter;
    public ValueConfigSearch epic_ShootFireNumberRelease;
    public ValueConfigSearch epic_ShootFireDmgMul;

    public ValueConfigSearch legendary_ShootFireDmgMul;
    [SerializeField] private BulletSimpleDamageObject bulletPrefab;

    private int currentShoot = 0;

    private void OnEnable()
    {
        if (Caster != null)
        {
            Caster.AttackHandler.onShoot -= OnShoot;
            Caster.AttackHandler.onShoot += OnShoot;
        }
    }
    public override void Play()
    {
        base.Play();
        Caster.AttackHandler.onShoot -= OnShoot;
        Caster.AttackHandler.onShoot += OnShoot;
    }
    public override void Remove()
    {
        base.Remove();
        Caster.AttackHandler.onShoot -= OnShoot;
    }

    private void OnDisable()
    {
        if (Caster != null)
        {
            Caster.AttackHandler.onShoot -= OnShoot;
        }
    }
    private void OnShoot(Character character)
    {
        if (Rarity < ERarity.Epic) return;

        currentShoot++;
        if (currentShoot % epic_ShootNumberCounter.IntValue == 0)
        {
            var dmg = 1f;
            if (Rarity >= ERarity.Epic)
            {
                dmg = epic_ShootFireDmgMul.FloatValue;
            }
            if (Rarity >= ERarity.Legendary)
            {
                dmg = legendary_ShootFireDmgMul.FloatValue;
            }

            Timing.RunCoroutine(_ShootRocket(dmg), gameObject);

        }
    }

    private IEnumerator<float> _ShootRocket(float dmg)
    {
        for (int i = 0; i < epic_ShootFireNumberRelease.IntValue; i++)
        {
            yield return Timing.WaitForSeconds(0.3f);
            var dmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * dmg);
            var speedStat = new Stat(WpEntity.BulletVelocity);

            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            var target = Caster.FindClosetTarget();
            if (target == null) break;

            bullet.transform.eulerAngles = new Vector3(0, 0, 1) * GameUtility.GameUtility.GetAngle(Caster, target);
            bullet.transform.position = Weapon.GetShootPoint().position;
            bullet.SetCaster(Caster);
            bullet.DmgStat = dmgStat;
            bullet.SetMaxHit(1);
            bullet.SetMaxHitToTarget(1);

            bullet.Movement.Speed = speedStat;
            bullet.Play();
        }
    }
}