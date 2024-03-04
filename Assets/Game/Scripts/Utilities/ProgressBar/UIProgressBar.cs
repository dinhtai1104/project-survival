using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressBar
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Image m_FilledPrgress;
        public void SetValue(float percent, bool animate)
        {
            m_FilledPrgress.fillAmount = percent;
        }
    }
}
