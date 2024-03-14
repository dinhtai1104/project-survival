using Engine;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.DamageObjects
{
    public class RicochetBullet2D : Bullet2D
    {
        protected override void OnImpact(Actor target)
        {
            base.OnImpact(target);
            // Find Next Target
            var allies = target.TargetFinder.Allies;

            var next = target.TargetFinder.CurrentQuery.GetTarget(allies, target);
            if (next == null)
            {
                Despawn();
                return;
            }

            Debug.Log("Ricochet to: " + next.gameObject.name, next.gameObject);
            var dir = next.CenterPosition - Trans.position;
            var angle = UnityEngine.Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var scaleY = Trans.localScale.y;
            if (angle > 90 || angle < -90)
            {
                Trans.LocalScaleY(-scaleY);
            }
            else
            {
                Trans.LocalScaleY(scaleY);
            }

            Trans.eulerAngles = new Vector3(0, 0, angle);
        }
    }
}
