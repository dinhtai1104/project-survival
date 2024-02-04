using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_1
{
    public class Boss10001Skill2Task : AnimationRequireSkillTask
    {
        public CharacterObjectBase weakendedCursePrefab;
        public Vector2 positionCurse;
        public override async UniTask Begin()
        {
            await base.Begin();
        }
        protected override void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
        {
            base.AnimationHandler_onEventTracking(trackEntry, e);
            if (trackEntry.TrackCompareAnimation(animationSkill) && e.EventCompare("attack_tracking"))
            {
                var weakendedCurse = PoolManager.Instance.Spawn(weakendedCursePrefab);
                weakendedCurse.transform.position = positionCurse;
                weakendedCurse.SetCaster(Caster);
                weakendedCurse.Play();
            }
        }
    }
}
