using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Utilities.Utilities.Scripts
{
    public class MoveFollowTarget : MonoBehaviour
    {
        private Transform m_Target;
        public void SetTarget(Transform target)
        {
            m_Target = target;
        }
        private void FixedUpdate()
        {
            if (m_Target == null) return;
            transform.position = m_Target.position;
        }
    }
}
