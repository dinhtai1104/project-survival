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
        [SerializeField] private Bound2D m_MapBound;

        public Bound2D MapBound { get => m_MapBound; set => m_MapBound = value; }
    }
}
