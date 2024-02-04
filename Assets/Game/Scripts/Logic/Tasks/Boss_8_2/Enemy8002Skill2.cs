using com.mec;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_2
{
    public class Enemy8002Skill2 : Boss_3_2_SkillTask
    {
        public ValueConfigSearch numberCircle;
        public ValueConfigSearch circleDelay;

        public override UniTask Begin()
        {
            numberCircle.SetId(Caster.gameObject.name);
            circleDelay.SetId(Caster.gameObject.name);
            return base.Begin();
        }
        protected override void ReleaseBullet()
        {
            Timing.RunCoroutine(_Shot(), gameObject);
        }

        private IEnumerator<float> _Shot()
        {
            for (int i = 0; i < numberCircle.IntValue; i++)
            {
                base.ReleaseBullet();
                yield return Timing.WaitForSeconds(circleDelay.FloatValue);
            }
        }
    }
}
