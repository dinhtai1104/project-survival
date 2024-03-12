using Assets.Game.Scripts.DataGame.Data;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Core.BuffHandler
{
    public class BuffGameObject : MonoBehaviour
    {
        private Actor m_Owner;
        private BuffData m_BuffData;
        public Actor Owner => m_Owner;
        public BuffData BuffData => m_BuffData;

        public void Init(Actor owner, BuffData buff)
        {
            m_BuffData = buff;
            m_Owner = owner;
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }
    }
}
