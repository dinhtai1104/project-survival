using Cysharp.Threading.Tasks;
using Game.Skill;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace InputController
{
    public class ButtonController : Controller
    {
        public Image img;
        public ButtonCoolDownMask coolDownMask;
        public GameObject inUseMask;
        public TMPro.TextMeshProUGUI text;

        CancellationTokenSource cancellation;
        private void OnEnable()
        {
            cancellation = new CancellationTokenSource();
            foreach (var action in actions)
            {
                if (action.texture != null)
                {
                    img.sprite = action.texture;
                }

                if (action.trackSkillTrigger != null)
                {
                    var skill = action.trackSkillTrigger.actor.SkillEngine.GetSkill(0) as MultiTaskSkill;
                    text.SetText($"x{(skill.maxCastTime.IntValue - skill.totalCast) }");
                }
                if (action.coolDownConfig != null)
                {
                    action.coolDownConfig.CheckForDeactive(
                           //when deactive
                           () =>
                           {
                               gameObject.SetActive(false);
                           },
                       cancellation).Forget();
                }

            }


        }
        public void Trigger()
        {
            if (!InputController.ENABLED || coolDownMask.IsCoolingDown || inUseMask.activeSelf) return;

            if (!canTrigger) return;
            int index = 0;
            foreach (var action in actions)
            {

                action.onTriggered?.Invoke();

                if (action.coolDownConfig != null)
                {
                    inUseMask.SetActive(true);
                    action.coolDownConfig.CheckForCoolDown(
                        //when cooldown ready
                        () =>
                    {
                        inUseMask.SetActive(false);
                        SetCoolDown();
                    },
                    cancellation).Forget();

                    action.coolDownConfig.CheckForDeactive(
                        //when deactive
                        () =>
                        {
                            gameObject.SetActive(false);
                        },
                    cancellation).Forget();
                }
                if (action.trackSkillTrigger != null)
                {
                    var skill = action.trackSkillTrigger.actor.SkillEngine.GetSkill(0) as MultiTaskSkill;
                    text.SetText($"x{(skill.maxCastTime.IntValue - skill.totalCast) + 1 }");
                }
            }
        }
      
        private void OnDestroy()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
        }
        public void Release()
        {
            if (!InputController.ENABLED) return;
            foreach (var action in actions)
            {
                action.onReleased?.Invoke();
            }
        }
        public void SetCoolDown()
        {
            foreach (var action in actions)
            {
                coolDownMask.StartCoolDown(action.coolDownConfig.skill );
            }
        }
        public void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}