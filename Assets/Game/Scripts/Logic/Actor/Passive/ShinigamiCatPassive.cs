using Game.GameActor.Buff;
using System;
using UnityEngine;

namespace Game.GameActor.Passive
{
    public class ShinigamiCatPassive : AbstractBuff
    {
        private ValueConfigSearch cooldown = new ValueConfigSearch("[ShinigamiHero]TimeImmuneCooldown", "30");
        private float timeCounting = 0;
        private float timeCooldown = 10f;
        private bool isImmune = false;
        private float timeImmuneCtr = 0;

        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.ImmuneAttack, OnAttackEvent);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.ImmuneAttack, OnAttackEvent);
        }

        private void OnAttackEvent(ActorBase attacker, ActorBase defender)
        {
            if (defender == Caster)
            {
                Caster.StatusEngine.ClearStatus<ImmuneStatus>();
                isImmune = false;
                timeCounting = 0;
            }
        }

        public override void Play()
        {
            timeCounting = 0;
            isImmune = true;
        }
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (isImmune)
            {
                ImmuneStatus();
            }
            else
            {
                //Caster.StatusEngine.ClearStatus<ImmuneStatus>();
                timeCounting += Time.deltaTime;
                if (timeCounting > cooldown.FloatValue)
                {
                    timeImmuneCtr = cooldown.FloatValue;
                    isImmune = true;
                }
            }

        }

        private async void ImmuneStatus()
        {
            timeCounting = 0;
           
            var status = await Caster.StatusEngine.AddStatus(Caster, EStatus.Immune, this);
            if (status == null) return;
            status.Init(Caster, Caster);
            status.SetDuration(10000);
        }
    }
}