using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.Btn;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Game.Scripts.GameScene.ControlView
{
    public class UIButtonSkill : UIBaseButton
    {
        private System.Action m_Callback;
        [SerializeField] private Text m_CooldownText;
        [SerializeField] private Image m_IconImage;
        [SerializeField] private Image m_CooldownProgressImage;

        private bool m_Interactable = true;
        private bool m_Empty;
        private bool m_Lock;

        public bool Empty
        {
            set
            {
                m_Empty = value;

                if (m_Empty)
                {
                    m_Interactable = button.interactable = false;
                    m_IconImage.enabled = false;
                }
                else
                {
                    m_Interactable = button.interactable = true;
                    m_IconImage.enabled = true;
                }
            }

            get => m_Empty;
        }

        public bool Lock
        {
            set
            {
                m_Lock = value;

                if (m_Lock)
                {
                    m_Interactable = button.interactable = false;
                    m_IconImage.enabled = false;
                }
                else
                {
                    m_Interactable = button.interactable = true;
                    m_IconImage.enabled = true;
                }
            }

            get => m_Lock;
        }

        public void SetCooldownText(float timer)
        {
            m_CooldownText.text = Mathf.CeilToInt(timer).ToString();
        }

        public void SetActiveCooldownProgress(bool active)
        {
            m_CooldownProgressImage.enabled = active;
        }

        public void SetInteractable(bool interactable)
        {
            m_Interactable = button.interactable = interactable;
            m_CooldownText.enabled = !interactable;
            m_IconImage.color = m_Interactable ? Color.white : Color.gray;
            m_CooldownProgressImage.enabled = !interactable;
        }

        public void SetCooldownProgress(float progress)
        {
            m_CooldownProgressImage.fillAmount = progress;
        }

        public void SetIcon(Sprite icon)
        {
            m_IconImage.sprite = icon;
        }


        public void SetCallback(Action callback)
        {
            m_Callback = callback;
        }
        public override void Action()
        {
            m_Callback?.Invoke();
        }
    }

    [System.Serializable]
    public class SkillAction
    {
        
    }
}
