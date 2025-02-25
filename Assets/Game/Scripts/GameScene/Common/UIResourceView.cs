using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class UIResourceView : MonoBehaviour
    {
        [SerializeField] private Image m_ResourceImg;
        [SerializeField] private Text m_ResourceAmount;

        public void SetSprite(Sprite sprite, bool isNativeSize = true)
        {
            m_ResourceImg.sprite = sprite;
            if (isNativeSize)
            {
                m_ResourceImg.SetNativeSize();
            }
        }
        public void SetAmount(int amount)
        {
            m_ResourceAmount.text = amount.TruncateValue();
        }
        public void SetAmount(long amount)
        {
            m_ResourceAmount.text = amount.TruncateValue();
        }
        public void SetAmount(float amount)
        {
            m_ResourceAmount.text = amount.TruncateValue();
        }
    }
}
