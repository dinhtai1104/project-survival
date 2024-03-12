using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UIBaseItemSlot : MonoBehaviour
    {
        public delegate void SlotOnClickItem(UIBaseItemSlot slot, UIGeneralIcon item);

        private UIGeneralIcon m_Icon;
        private SlotOnClickItem m_CallbackOnClicked;
        [SerializeField] private RectTransform m_IconParent;
        [SerializeField] private Button m_Button;

        public UIGeneralIcon Icon { get => m_Icon; set => m_Icon = value; }

        private void OnEnable()
        {
            m_Button.onClick.AddListener(InventorySlotOnClick);
        }
        private void OnDisable()
        {
            m_Button.onClick.RemoveListener(InventorySlotOnClick);
        }

        public void Clear()
        {
            if (m_Icon != null)
            {
                PoolFactory.Despawn(m_Icon.gameObject);
                m_Icon = null;
            }
        }

        public void SetIcon(UIGeneralIcon icon)
        {
            Clear();
            m_Icon = icon;
            m_Icon.transform.SetParentUI(m_IconParent);
        }


        private void InventorySlotOnClick()
        {
            m_CallbackOnClicked?.Invoke(this, Icon);
        }

    }
}
