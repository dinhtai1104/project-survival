using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class RangeShotOverwriteAnimationFixAim : RangeShotOverwriteAnimation
    {
        protected override BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
        {
            return base.ReleaseBullet(pos, 180);
        }
    }
}
