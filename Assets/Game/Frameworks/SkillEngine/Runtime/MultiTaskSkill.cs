using Game.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.GameActor;

namespace Game.Skill
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TaskRunner))]
    public class MultiTaskSkill : BaseSkill
    {
        [SerializeField, Title("Cooldown Skill Config")] private ValueConfigSearch cooldownSkillValue;
        [SerializeField, Title("Cooldown Skill Config")] public ValueConfigSearch maxCastTime;
        [SerializeField] private TaskRunner _taskRunner;
        public Task CurrentTask { get { return _taskRunner.CurrentTask; } }
        public Task[] Tasks { get { return _taskRunner.Tasks; } }
        public BaseSkill skillConnectCooldownWhenSkillCompleted;
        private void OnValidate()
        {
            _taskRunner = GetComponent<TaskRunner>();
        }
        protected override void Start()
        {

            base.Start();
        }
        public override void Initialize(ActorBase actor)
        {
            try
            {
                _cooldown.BaseValue = cooldownSkillValue.SetId(actor.gameObject.name).FloatValue * actor.Stats.GetValue(StatKey.Cooldown, 1);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            base.Initialize(actor);
        }
        public override void Cast()
        {
            if (_taskRunner.StartingTask == null || !_taskRunner.StartingTask.HasTask())
            {
                OnCompleteSkill();
                return;
            }
            int maxCast = maxCastTime.IntValue;
            if (maxCast != 0 && totalCast >= maxCast)
            {

                return;
            }
            totalCast++;
            base.Cast();
            _taskRunner?.RunTask();
            _taskRunner.OnComplete -= OnCompleteSkill;
            _taskRunner.OnComplete += OnCompleteSkill;

            Messenger.Broadcast(EventKey.OnCastSkill, Caster, totalCast-1, maxCast);

        }

        public override void Stop()
        {
            base.Stop();
            _taskRunner?.StopTask();
            _taskRunner.OnComplete -= OnCompleteSkill;
        }
        private void OnCompleteSkill()
        {
            _taskRunner.OnComplete -= OnCompleteSkill;
            IsExecuting = false;
            StartCooldown();
            skillConnectCooldownWhenSkillCompleted?.StartCooldown();
        }
    }
}