using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Utilities
{
    public static class SpineExtension
    {
        public static bool TrackCompareAnimation(this Spine.TrackEntry track, string animation)
        {
            return track.Animation.Name == animation;
        }
        public static bool EventCompare(this Spine.Event e, string eventS) 
        {
            return e.Data.Name == eventS;
        }

    }
}
