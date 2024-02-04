using Game.Skill;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCoolDownMask : MonoBehaviour
{
    [SerializeField]
    private Image maskImg;
    [SerializeField]
    private TMPro.TextMeshProUGUI coolDownText;
    [SerializeField]
    private MMF_Player coolDownCompleteFb;
    ISkill skill;
    public void StartCoolDown(ISkill skill)
    {
        this.skill = skill;
        maskImg.enabled = true;
        maskImg.fillAmount = 1;
    }
    public bool IsCoolingDown { get => maskImg.enabled; }
    float t = 0;
    private void Update()
    {
        if (!maskImg.enabled) return;
        if (skill.IsCoolingDown)
        {

            //get timeleft
            float timer = skill.GetCoolDown()-skill.CoolDownTimer;
            maskImg.fillAmount = timer/skill.GetCoolDown();
            if (t > 0)
            {
                t -= Time.deltaTime;
            }
            else
            {
                t = 1f;
                coolDownText.text = ((int)timer).ToString();
            }
        }
        else
        {
            coolDownText.text = string.Empty;

            maskImg.enabled = false;
            coolDownCompleteFb?.PlayFeedbacks();
        }
    }
}
