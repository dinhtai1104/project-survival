using com.mec;
using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_2
{
    [CreateAssetMenu(menuName = "CharacterBehaviours/PunchFromSkyBehaviour")]
    public class PunchFromSkyBehaviour : CharacterBehaviour
    {
        public GameObject punchPrefab;
        public GameObject predict;
        public ValueConfigSearch cooldown;
        public ValueConfigSearch punchPosY;
        public ValueConfigSearch punchSize;
        public ValueConfigSearch punchDmg;
        public ValueConfigSearch punchVelocity;
        public ValueConfigSearch punchNumber;
        public ValueConfigSearch punchDelay;
        public ValueConfigSearch predictDelayBeforeShot;
        public LayerMask groundMask;
        private float cooldownTimer = 0;
        private bool isExecute = false;
        private ActorBase actor;
        private BulletSimpleDamageObject punchCache;
        private GameObject predictCache;
        public override CharacterBehaviour SetUp(ActorBase character)
        {
            var bh = (PunchFromSkyBehaviour)base.SetUp(character);

            bh.punchPrefab = punchPrefab;
            bh.predict = predict;
            bh.cooldown = cooldown.Clone();
            bh.punchPosY = punchPosY;
            bh.punchSize = punchSize.Clone();
            bh.punchDmg = punchDmg.Clone();
            bh.punchVelocity = punchVelocity.Clone();
            bh.punchNumber = punchNumber.Clone();
            bh.punchDelay = punchDelay.Clone();
            bh.groundMask = groundMask;
            bh.predictDelayBeforeShot = predictDelayBeforeShot.Clone();

            return bh;
        }
        public override void OnActive(ActorBase character)
        {
            base.OnActive(character);
            cooldown.SetId(character.gameObject.name);
            punchSize.SetId(character.gameObject.name);
            punchDmg.SetId(character.gameObject.name);
            punchVelocity.SetId(character.gameObject.name);
            punchDelay.SetId(character.gameObject.name);
            punchNumber.SetId(character.gameObject.name);
            active = true;
            punchCache = this.punchPrefab.GetComponent<BulletSimpleDamageObject>();
            predictCache = this.predict;
            actor = character;
            cooldownTimer = 0;
        }
        public override void OnUpdate(ActorBase character)
        {
            if (character.IsDead() || isExecute || !predictCache || !punchCache) return;

            base.OnUpdate(character);
            if (cooldownTimer > cooldown.FloatValue)
            {
                cooldownTimer = 0;
                isExecute = true;
                RunSkill();
            }
            else
            {
                cooldownTimer += Time.deltaTime;
            }
        }

        private void RunSkill()
        {
            Timing.RunCoroutine(_ShotPunch(), "Punch");
        }
        public override void OnDeactive(ActorBase character)
        {
            base.OnDeactive(character);
            Timing.KillCoroutines("Punch");
        }
        public override void OnDead(ActorBase character)
        {
            base.OnDead(character);
            Timing.KillCoroutines("Punch");
        }

        private IEnumerator<float> _ShotPunch()
        {
            for (int i = 0; i < punchNumber.IntValue; i++)
            {
                var target = actor.FindClosetTarget(); 
                if (target == null)
                {
                    isExecute = false;
                    break;
                }
                // spawn
                var predict = PoolManager.Instance.Spawn(predictCache);

                var rd = UnityEngine.Random.insideUnitCircle * 2;
                rd.y = 0;
                var pos = (Vector2)target.GetPosition() + rd;

                var rch = Physics2D.Raycast(pos, Vector3.down, Mathf.Infinity, groundMask);
                if (rch.collider)
                {
                    pos = rch.point + Vector2.up * 0.2f;
                }

                predict.transform.position = pos;

                pos.y = punchPosY.FloatValue;

                yield return Timing.WaitForSeconds(predictDelayBeforeShot.FloatValue);

                var bullet = PoolManager.Instance.Spawn(punchCache);
                bullet.transform.position = pos;

                bullet.transform.localScale = Vector3.one * punchSize.FloatValue;

                var statSpeed = new Stat(punchVelocity.FloatValue);

                var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
                Messenger.Broadcast(EventKey.PreFire, actor, (BulletBase)null, listModi);
                bullet.Movement.Speed = statSpeed;
                bullet.transform.eulerAngles = Vector3.forward * -90;
                bullet.SetCaster(actor);
                bullet.DmgStat = new Stat(actor.Stats.GetValue(StatKey.Dmg) * punchDmg.SetId(actor.gameObject.name).FloatValue);
                bullet.SetMaxHit(1);
                bullet.SetMaxHitToTarget(1);
                bullet.Play();
                bullet.Movement.Move(statSpeed, Vector3.down);
                bullet.onBeforeDestroy += (a) =>
                {
                    PoolManager.Instance.Despawn(predict);
                };
                yield return Timing.WaitForSeconds(punchDelay.FloatValue);
            }
            isExecute = false;
        }
    }
}
