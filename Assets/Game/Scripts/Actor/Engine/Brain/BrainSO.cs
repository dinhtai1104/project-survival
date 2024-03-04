using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine
{
    [CreateAssetMenu(fileName = "BrainSO.asset", menuName = "Brain/AI/BrainSO", order = -1)]
    public class BrainSO : ScriptableObject
    {
        public BrainTransition[] m_GlobalTransitions;
        public BrainLocalTransition[] m_LocalTransitions;
    }
}
