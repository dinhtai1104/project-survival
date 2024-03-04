using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IService
    {
        void OnStart();
        void OnInit();
        void OnDispose();
        void OnUpdate();
    }
}

namespace Core
{
    public abstract class Service : IService
    {
        public static NullService Null = new NullService();

        public virtual void OnStart()
        {
            Logger.Log("Service " + GetType() + " On Start", Color.yellow);
        }
        public virtual void OnDispose()
        {
            Logger.Log("Service " + GetType() + " On Dispose", Color.red);
        }
        public virtual void OnInit()
        {
            Logger.Log("Service " + GetType() + " On Init", Color.blue);
        }
        public virtual void OnUpdate()
        {
        }
    }
}

namespace Core
{
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

        public void OnUpdate()
        {
        }
    }
}
