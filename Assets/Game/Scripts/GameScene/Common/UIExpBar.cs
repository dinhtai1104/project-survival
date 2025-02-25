using ProgressBar;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class UIExpBar : UIProgressBar
    {
        [SerializeField] private Text m_LevelTxt;
        public void SetLevel(int level)
        {
            m_LevelTxt.text = level.ToString();
        }
    }
}
