using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Utilities.UITween
{
    public class UITweenTriggerAnimator : UITweenBase
    {
        public Animator animator;
        public string trigger;
        public async override UniTask Hide()
        {

        }

        public async override UniTask Show()
        {
            animator.SetTrigger(trigger);
        }
    }
}
