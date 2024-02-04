using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_1
{
    public class ChargeFindDirectionTargetHorizontalTask : SkillTask
    {
        public Vector2 chargeDirection;
        public Transform chargeHolder;
        public GameObject chargePrefab;
        public ValueConfigSearch chargeTime;
        public ValueConfigSearch chargeRayLength;

        private float time = 0;
        private GameObject chargeObj;

        public override async UniTask Begin()
        {
            chargeTime.SetId(Caster.gameObject.name);
            chargeRayLength.SetId(Caster.gameObject.name);

            await base.Begin();
            time = 0;
            chargeObj = PoolManager.Instance.Spawn(chargePrefab, chargeHolder);
            chargeObj.transform.localPosition = Vector3.zero;
            chargeObj.transform.localScale = new Vector3(chargeRayLength.FloatValue, 1, 1);
        }

        public override void Run()
        {
            base.Run();
            time += Time.deltaTime;
            if (time > chargeTime.FloatValue)
            {
                IsCompleted = true;
                return;
            }
            UpdateCharge();
        }

        public override void OnStop()
        {
            if (chargeObj != null)
            {
                PoolManager.Instance.Despawn(chargeObj);
                chargeObj = null;
            }
            base.OnStop();
        }
        public override UniTask End()
        {
            if (chargeObj != null)
            {
                PoolManager.Instance.Despawn(chargeObj);
                chargeObj = null;
            }
            return base.End();
        }

        private void UpdateCharge()
        {
            var main = Caster.FindClosetTarget();
            if (main == null)
            {
                Skill.Stop();
                IsCompleted = true;
                return;
            }
            Caster.SetFacing(main as ActorBase);
        }
    }
}
