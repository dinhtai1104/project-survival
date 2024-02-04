using Game.Effect;
using Game.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_1
{
    public class TheWeakenedCurseObject : CharacterObjectBase
    {
        [SerializeField] private ValueConfigSearch hpDecrease;
        public string VFXCurse = "";
        private StatModifier modifier;
        public override void Play()
        {
            base.Play();
            var mainPlayer = GameController.Instance.GetMainActor();
            if (mainPlayer != null)
            {
                var currentHp = mainPlayer.HealthHandler.GetHealth() * hpDecrease.FloatValue;
                mainPlayer.HealthHandler.AddHealth(-currentHp / 2);
                EffectHandler.GetEffect(VFXCurse, t =>
                {
                    t.GetComponent<EffectAbstract>().Active(mainPlayer.GetMidPos());
                });
            }
        }
    }
}
