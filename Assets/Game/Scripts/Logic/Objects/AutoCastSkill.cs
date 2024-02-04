using Game.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Objects
{
    public class AutoCastSkill : MonoBehaviour, IUpdate
    {
        private ISkillEngine skill;
        public int index;
        private void Awake()
        {
            skill = GetComponent<ISkillEngine>();
        }

        public void OnInit()
        {
        }

        public void OnUpdate()
        {
            if (skill == null) return;
            if (skill.GetSkill(0).CanCast)
            {
                skill.CastSkill(0);
            }
        }
    }
}
