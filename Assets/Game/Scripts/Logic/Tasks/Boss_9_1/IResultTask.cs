using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_1
{
    public interface IResultTask
    {
        T Get<T>();
    }
    public interface IResultTask<T> : IResultTask
    {
        T Result { get; }
    }
}
