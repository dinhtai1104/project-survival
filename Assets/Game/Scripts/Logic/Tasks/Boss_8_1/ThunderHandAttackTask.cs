using com.mec;
using Game.GameActor;
using Game.Pool;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_1
{
    public class ThunderHandAttackTask : RangeShotTask
    {
        [SerializeField] private Transform leftHand, rightHand;


        [Header("Left-Hand")]
        public ValueConfigSearch leftHand_UpperAngle;
        public ValueConfigSearch leftHand_LowerAngle;

        [Header("Right-Hand")]
        public ValueConfigSearch rightHand_UpperAngle;
        public ValueConfigSearch rightHand_LowerAngle;


        protected override void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
        {
            if (trackEntry.Animation.Name == animationSkill)
            {
                if (e.Data.Name == "attack_tracking")
                {
                    base.OnAttackTracking(leftHand);
                    base.OnAttackTracking(rightHand);
                }
            }
        }
        protected override IEnumerator<float> _Shot(Transform pos)
        {
            ValueConfigSearch upper, lower;
            if (pos == leftHand)
            {
                upper = leftHand_UpperAngle;
                lower = leftHand_LowerAngle;
            }
            else
            {
                upper = rightHand_UpperAngle;
                lower = rightHand_LowerAngle;
            }
            var target = Caster.FindClosetTarget();
            if (target == null)
            {
                target = GameController.Instance.GetMainActor();
            }
            if (target != null)
            {
                for (int i = 0; i < (bulletNumber.IntValue == 0 ? 1 : bulletNumber.IntValue); i++)
                {
                    var angle = GameUtility.GameUtility.GetRandomAngle(upper.FloatValue + Caster.GetFacingDirection() * 180, lower.FloatValue + Caster.GetFacingDirection() * 180);
                    ReleaseBullet(pos, angle);
                    yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
                }
            }
        }

        private void OnDrawGizmos()
        {
            var caster = GetComponentInParent<ActorBase>();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftHand.position, leftHand.position + GetDir(leftHand_UpperAngle.FloatValue + caster.transform.eulerAngles.y) * 3);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftHand.position, leftHand.position + GetDir(leftHand_LowerAngle.FloatValue + caster.transform.eulerAngles.y) * 3);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(rightHand.position, rightHand.position + GetDir(rightHand_UpperAngle.FloatValue + caster.transform.eulerAngles.y) * 3);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(rightHand.position, rightHand.position + GetDir(rightHand_LowerAngle.FloatValue + caster.transform.eulerAngles.y) * 3);
        }

        public Vector3 GetDir(float angle)
        {
            return new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        }
    }
}
