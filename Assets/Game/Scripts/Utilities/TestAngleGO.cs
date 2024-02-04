using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Game.Scripts.Utilities
{
    public class TestAngleGO : MonoBehaviour
    {
        public float angle;
        [Button]
        public void TestAngle()
        {
            transform.eulerAngles = angle * Vector3.forward;
        }
    }
}
