using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressBar
{
    public enum ProgressType
    {
        FillAmount,
        SizeRect,
    }
    public enum DirectionType
    {
        Horizontal,
        Vertical,
    }
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Image m_FilledPrgress;
        [SerializeField] private ProgressType m_ProgressType = ProgressType.FillAmount;
        [SerializeField, ShowIf("m_ProgressType", ProgressType.SizeRect)] private DirectionType m_Direction = DirectionType.Horizontal;

        [SerializeField, ShowIf("m_ProgressType", ProgressType.SizeRect)] private float m_CacheMaxSize = 0;
        public void SetValue(float percent, bool animate = false)
        {
            if (m_ProgressType == ProgressType.FillAmount)
            {
                var last = m_FilledPrgress.fillAmount;
                //m_FilledPrgress.fillAmount = percent;
                m_FilledPrgress.DOKill();
                m_FilledPrgress.DOFillAmount(percent, 0.2f).From(last);
            }
            else
            {
                var currentSize = m_FilledPrgress.rectTransform.sizeDelta;
                var lastSize = currentSize;
                if (m_Direction == DirectionType.Horizontal)
                {
                    currentSize.x = m_CacheMaxSize * percent;
                }
                else
                {
                    currentSize.y = m_CacheMaxSize * percent;
                }

                m_FilledPrgress.rectTransform.DOKill();
                m_FilledPrgress.rectTransform.DOSizeDelta(currentSize, 0.2f).From(lastSize);
            }
        }
    }
}
