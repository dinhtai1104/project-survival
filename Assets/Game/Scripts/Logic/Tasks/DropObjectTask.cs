using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class DropObjectTask : RangeShotTask
    {
        protected override void OnCompleteTracking(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == animationSkill)
            {
                eventShoot?.Invoke();
                ReleaseBullet(pos);
            }
            base.OnCompleteTracking(trackEntry);
        }
        protected override void OnEventTracking(TrackEntry trackEntry, Event e)
        {
            //base.OnEventTracking(trackEntry, e);
        }
    }
}
