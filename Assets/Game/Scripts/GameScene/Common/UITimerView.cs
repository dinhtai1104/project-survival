using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class UITimerView : MonoBehaviour
    {
        [SerializeField] private Text m_TimerTxt;
        private string m_PrefixTxt = "";

        public void SetPrefixTxt(string prefixTxt)
        {
            m_PrefixTxt = prefixTxt;
        }

        public void SetTimer(TimeSpan timeSpan)
        {
            m_TimerTxt.text = m_PrefixTxt + timeSpan.ConvertMinuteSecond();
        }

        public void SetTimer(string timer)
        {
            m_TimerTxt.text = m_PrefixTxt + timer;
        }
    }
}
