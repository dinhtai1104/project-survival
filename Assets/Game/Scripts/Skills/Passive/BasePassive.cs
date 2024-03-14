using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Skills.Passive
{
    public class BasePassive : MonoBehaviour, IPassive
    {
        public Actor Owner { get; set; }

        public virtual void Equip()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void UnEquip()
        {
        }
    }
}
