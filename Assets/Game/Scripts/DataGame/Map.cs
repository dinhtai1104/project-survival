using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Collider2D m_CameraBoundaries;
        [SerializeField] private Bound2D m_MapBound;
        [SerializeField] private Bound2D m_SpawnBound;

        public Bound2D MapBound =>m_MapBound;
        public Bound2D SpawnBound => m_SpawnBound;
        public Collider2D CameraBoundaries => m_CameraBoundaries;


        private void OnDrawGizmos()
        {
            m_MapBound.DrawBounds(Color.white);
            m_SpawnBound.DrawBounds(Color.green);
        }
    }
}
