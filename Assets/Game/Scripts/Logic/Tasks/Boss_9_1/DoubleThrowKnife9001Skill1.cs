using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_1
{
    public class DoubleThrowKnife9001Skill1 : RangeShotConeSide
    {
        public Transform pos2;
        protected override void OnEventAttrach()
        {
            // Angle to target
            var angleToTarget = GetAngleToTarget(pos);
            var dir = GameUtility.GameUtility.ConvertDir(angleToTarget);
            base.OnEventAttrach();


            // Angle to oppsite
            var dirReflect = Vector2.Reflect(dir, dir.y <= 0.5f ? Vector3.right : Vector3.up);
            var angle = GameUtility.GameUtility.GetAngle(dirReflect);
            SpawnEffect(pos2);
            SpawnCone(pos2, angle);
        }
    }
}
