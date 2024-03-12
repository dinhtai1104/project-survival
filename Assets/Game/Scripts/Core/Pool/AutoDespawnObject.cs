using com.mec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pool
{
    public class AutoDespawnObject : MonoBehaviour
    {
        [SerializeField] private float m_TimeLive = 5;
        private void OnEnable()
        {
            Timing.RunCoroutine(_Killing(), gameObject);
        }

        private IEnumerator<float> _Killing()
        {
            yield return Timing.WaitForSeconds(m_TimeLive);
            PoolFactory.Despawn(gameObject);
        }
        public void Stop()
        {
            Timing.KillCoroutines(gameObject);
        }
    }
}
