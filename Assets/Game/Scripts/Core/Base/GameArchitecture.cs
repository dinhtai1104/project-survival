using com.sparkle.core;
using Cysharp.Threading.Tasks;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        public new static TService GetService<TService>() where TService : IService
        {
            return Instance.GetService<TService>();
        }
    }
}
