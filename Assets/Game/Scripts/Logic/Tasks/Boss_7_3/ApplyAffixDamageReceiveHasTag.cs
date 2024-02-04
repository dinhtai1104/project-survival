using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_7_3
{
    public class ApplyAffixDamageReceiveHasTag : MonoBehaviour
    {
        public ETag eTag;
        public EStatMod stat;
        public ValueConfigSearch dmgIncrease;
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, BeforeHit);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, BeforeHit);
        }

        private void BeforeHit(ActorBase atk, ActorBase def, DamageSource dmgs)
        {
            var actor= GetComponent<ActorBase>();
            if (actor == null) return;
            if (actor == def && def.Tagger.HasAnyTags(eTag))
            {
                dmgs._damage.AddModifier(new StatModifier(stat, dmgIncrease.FloatValue));
            }
        }
    }
}
