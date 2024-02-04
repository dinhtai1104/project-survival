using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_3
{
    public class HitActionSpawnFirePillar : MonoBehaviour, IHitTriggerAction
    {
        public CharacterObjectBase Base;
        public BulletSimpleDamageObject fireFillar;
        public LayerMask ground;

        public ValueConfigSearch pillarDmg;
        public ValueConfigSearch pillarDmgInterval;
        public ValueConfigSearch pillarDuration;

        public void Action(Collider2D collider)
        {
            Vector3 rotation = Vector3.zero;
            Vector3 pos = Vector3.zero;
            var target = collider.GetComponentInParent<ITarget>();
            if (target == null)
            {
                // Ground
                pos = transform.position;
                var dir = transform.right;
                var lastPosTrigger = pos - dir * 1.2f;

                var rch = Physics2D.Raycast(lastPosTrigger, dir, 100, ground);
                var pillar = PoolManager.Instance.Spawn(fireFillar);
                pillar.SetCaster(Base.Caster);
                pillar.DmgStat = new Stat(Base.Caster.GetStatValue(StatKey.Dmg) * pillarDmg.FloatValue);
                pillar.SetMaxHit(99999);
                pillar.SetMaxHitToTarget(99999);
                pillar._hit.SetIntervalTime(pillarDmgInterval.FloatValue);
                pillar.GetComponent<AutoDestroyObject>().SetDuration(pillarDuration.FloatValue);
                pillar.transform.position = transform.position;
                pillar.transform.up = rch.normal;
                pillar.Play();
            }
            if ((ActorBase)target == Base.Caster) return;
        }
    }
}
