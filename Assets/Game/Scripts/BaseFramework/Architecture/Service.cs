using Assets.Game.Scripts.BaseFramework.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.BaseFramework.Architecture
{
    public interface IService
    {
        void OnStart();
        void OnInit();
        void OnDispose();
    }
    public abstract class Service : IService
    {
        public static NullService Null = new NullService();

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

    public class NullService : IService
    {
        public void OnDispose()
        {
        }

        public void OnInit()
        {
        }

        public void OnStart()
        {
        }
    }
}
