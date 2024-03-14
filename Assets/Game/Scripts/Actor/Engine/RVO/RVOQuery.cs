using RVO;
using System;
using UnityEngine;

namespace Engine
{
    public class RVOQuery : MonoBehaviour, IRVO
    {
        public int Id => m_Id;
        private Actor m_Owner;
        private int m_Id = -1;
        public void Init(Actor owner)
        {
            m_Owner = owner;
            Simulator.Instance.delAgent(m_Id);
            m_Id = Simulator.Instance.addAgent(new Vector2RVO(m_Owner.CenterPosition));
        }

        public Vector2 NextPosition()
        {
            if (m_Id < 0) ReInit();
            if (m_Id >= 0)
            {
                Vector2RVO pos = Simulator.Instance.getAgentPosition(m_Id);
                Vector2RVO vel = Simulator.Instance.getAgentPrefVelocity(m_Id);
                return new Vector2(pos.x(), pos.y());
            }
            return Vector2.zero;
        }

        public void OnUpdate()
        {
            

        }

        public void ReInit()
        {
            if (m_Id >= 0)
            {
                Simulator.Instance.delAgent(m_Id);
            }
            m_Id = Simulator.Instance.addAgent(new Vector2RVO(m_Owner.CenterPosition));
        }

        private void OnDisable()
        {
            m_Id = -1;
        }
    }
}
