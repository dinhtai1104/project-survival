using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class AnimationRequireSkillTask : SkillTask
    {
        public int trackAnim = 0;
        public string animationSkill;
        public override async UniTask Begin()
        {
            await base.Begin();
            if (animationSkill.IsNotNullAndEmpty())
            {
                Caster.AnimationHandler.SetAnimation(trackAnim, animationSkill, false);
            }
            Caster.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
            Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;
        }

        protected virtual void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
        {

        }

        protected virtual void AnimationHandler_onCompleteTracking(Spine.TrackEntry trackEntry)
        {
            if (trackEntry.TrackCompareAnimation(animationSkill))
            {
                IsCompleted = true;
            }
        }

        public override UniTask End()
        {
            Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
            Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
            return base.End();
        }
        public override void OnStop()
        {
            Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
            Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
            base.OnStop();
        }
    }
}
