using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks
{
    public class EnableColliderTask : Task
    {
        [SerializeField] private Collider2D m_Collider2D;
        [SerializeField] private bool m_Value;
        public override void Begin()
        {
            base.Begin();
            m_Collider2D.enabled = m_Value;
            IsCompleted = true;
        }

    }
}
