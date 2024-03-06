using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneManger
{
    public interface ISceneController
    {
        bool IsInitialized { get; }
        bool IsEnter { get; }
        void Init();
        UniTask RequestAssets();

        void Enter();
        UniTask Exit(bool reload);
        void Execute();
        void Focus();
        void Unfocus();
    }
}
