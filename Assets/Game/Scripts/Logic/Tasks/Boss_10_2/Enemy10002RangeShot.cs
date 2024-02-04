using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_2
{
    public class Enemy10002RangeShot : RangeShotTask
    {
        public CirclePatternBulletSpawner miniBulletPaternPrefab;
        public ValueConfigSearch miniBullet_RotateSpeed;
        public ValueConfigSearch miniBullet_Dmg;
        public ValueConfigSearch miniBullet_Size;
        public ValueConfigSearch miniBullet_Number;
        public ValueConfigSearch miniBullet_Distance;

        public override UniTask Begin()
        {
            miniBullet_RotateSpeed.SetId(Caster.gameObject.name);
            miniBullet_Dmg.SetId(Caster.gameObject.name);
            miniBullet_Size.SetId(Caster.gameObject.name);
            miniBullet_Number.SetId(Caster.gameObject.name);
            miniBullet_Distance.SetId(Caster.gameObject.name);

            return base.Begin();
        }

        protected override BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
        {
            var MainBullet =  base.ReleaseBullet(pos, angle);

            var pattern = PoolManager.Instance.Spawn(miniBulletPaternPrefab);
            pattern.transform.position = pos.position;
            //pattern.GetComponent<AutoFollowObject>().SetFollow(MainBullet.transform);
            pattern.SpawnItem(miniBullet_Number.IntValue, miniBullet_Size.FloatValue, OnSpawnBullet);
            pattern.Rotate.Speed = new Stat(miniBullet_RotateSpeed.FloatValue);
            pattern.Movement.SetMove(GameUtility.GameUtility.ConvertDir(angle));
            pattern.Movement.Speed = MainBullet.Movement.Speed;
            pattern.transform.eulerAngles = Vector3.forward * angle;

            pattern.Play();
            return MainBullet;
        }

        private void OnSpawnBullet(BulletSimpleDamageObject obj)
        {
            obj.SetCaster(Caster);
            obj.SetMaxHit(1);
            obj.SetMaxHitToTarget(1);
            obj.transform.position += obj.transform.right * miniBullet_Distance.FloatValue;
            obj.DmgStat = new Stat(Caster.GetStatValue(StatKey.Dmg) * miniBullet_Dmg.FloatValue);
            obj.Play();
        }
    }
}
