using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UIGeneralIcon : UIBaseIcon
    {
        [SerializeField] private Image m_Frame;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Text m_Amount;

        public void SetValueAmount(int amount)
        {
            if (m_Amount != null)
            {
                if (amount <= 0) m_Amount.text = string.Empty;
                m_Amount.text = amount.TruncateValue();
            }
        }

        public void SetSpriteFrame(Sprite sprite)
        {
            if (m_Frame != null)
            {
                m_Frame.sprite = sprite;
            }
        }
        public void SetSpriteIcon(Sprite sprite)
        {
            if (m_Icon != null)
            {
                m_Icon.sprite = sprite;
            }
        }
    }
}
