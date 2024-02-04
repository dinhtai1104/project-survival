using Cysharp.Threading.Tasks;
using Game.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Game.Scripts.Logic.Tasks
{
    public class UITaskHideTweenUI : Task
    {
        public UITweenRunner tween;
        public override async UniTask Begin()
        {
            await base.Begin();
            tween.Close();

            IsCompleted = true;
        }
    }
}
