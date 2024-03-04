using Cysharp.Threading.Tasks;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GameArchitecture : Architecture
    {
        protected override void Awake()
        {
            base.Awake();
        }
        public override async UniTask Init()
        {
            await UniTask.Yield();
            Register(DataManager.Instance);

            await UniTask.Yield();
            Register<TaskRunnerMgr>();

            await UniTask.Yield();
            Register<EventMgr>();

            await UniTask.Yield();
            DataManager.Save.OnLoaded();

            await UniTask.Yield();
            DataManager.Save.FixData();

            await UniTask.Yield();
            Register<ShortTermMemoryService>();
        }
    }
}
