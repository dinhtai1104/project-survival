using Assets.Game.Scripts.BaseFramework.Architecture;
using UnityEngine;

public class ShortTermMemoryService : ShortTermMemory, IService
{
    public virtual void OnStart()
    {
        Logger.Log("Service " + this.GetType() + " On Start", Color.yellow);
    }
    public virtual void OnDispose()
    {
        Logger.Log("Service " + this.GetType() + " On Dispose", Color.red);
    }
    public virtual void OnInit()
    {
        Logger.Log("Service " + this.GetType() + " On Init", Color.blue);
    }
}
