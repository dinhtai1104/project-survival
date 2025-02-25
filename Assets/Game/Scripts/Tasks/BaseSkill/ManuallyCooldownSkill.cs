using Engine;
using UnityEngine;
namespace Assets.Game.Scripts.Tasks.BaseSkill
{
    public class ManuallyCooldownSkill : MultiTaskSkill
    {
        [SerializeField] protected BindConfig m_CooldownConfig = new("[{0}]Skill_Cooldown", 2);

        protected override void OnInit()
        {
            m_CooldownConfig.SetId(Caster.gameObject.name);
            base.OnInit();

            Cooldown = m_CooldownConfig.FloatValue;
        }
    }
}
