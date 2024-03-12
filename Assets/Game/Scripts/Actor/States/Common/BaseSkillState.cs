using com.mec;
using Engine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine.State.Common
{
    public class BaseSkillState : BaseActorState
    {
        [SerializeField] private Stat _cooldown;

        [ReadOnly] private StatModifier _cooldownModifier;

        public float Cooldown
        {
            set => _cooldown.BaseValue = value;
            get => _cooldown.Value;
        }

        public int SkillId { set; get; }
        public bool IsCooldowning { private set; get; }
        public float CooldownTimer => Mathf.Clamp(_cooldownTimer, 0f, Cooldown);
        public float RemaningCooldownProgress => CooldownTimer / Cooldown;

        public bool DisableCooldown { protected set; get; }

        private float _cooldownTimer;

        public override void Enter()
        {
            base.Enter();
            _cooldown.RecalculateValue();
            if (!DisableCooldown && Cooldown > 0f)
            {
                StartCooldowning();
            }
        }

        private IEnumerator<float> _Cooldowning()
        {
            while (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                yield return 0f;
            }

            IsCooldowning = false;
        }

        protected void StartCooldowning()
        {
            _cooldownTimer = Cooldown;
            IsCooldowning = true;
            Timing.RunCoroutine(_Cooldowning());
        }

        [Button]
        public void ResetCoolDown(float cooldownTime)
        {
            Cooldown = cooldownTime;
            _cooldownTimer = 0;
        }
    }
}
