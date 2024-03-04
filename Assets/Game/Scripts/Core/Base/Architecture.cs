using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using SRDebugger.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IAchitecture
    {
        void Register<TService>() where TService : class, IService;
        void Register(IService service);
        TService GetService<TService>() where TService : class, IService;
        void StartServices();
        void DisposeService();
    }
}

namespace Core
{
    public abstract class Architecture : LiveSingleton<Architecture>, IAchitecture
    {
        public static bool IsSetup = false;
        [ShowInInspector]
        protected Dictionary<Type, IService> m_LookupServices = new Dictionary<Type, IService>();
        private List<IService> m_Services = new List<IService>();

        protected virtual void InitServices()
        {
        }
        public abstract UniTask Init();
        public static TService Get<TService>() where TService : class, IService
        {
            return Instance.GetService<TService>();
        }
        public static bool ContainsService<TService>() where TService : class, IService
        {
            return Instance.Contains<TService>();
        }
        private bool Contains<TService>() where TService : class, IService
        {
            return m_LookupServices.ContainsKey(typeof(TService));
        }
        public void Register<TService>() where TService : class, IService
        {
            var type = typeof(TService);
            if (!m_LookupServices.ContainsKey(type))
            {
                var serviceInstance = Activator.CreateInstance(type) as IService;
                Logger.Log("Add service {" + type + "}");
                m_LookupServices.Add(type, serviceInstance);
                m_Services.Add(serviceInstance);
                serviceInstance.OnInit();
            }
            else
            {
                Logger.LogError("Service {" + type + "} already exist in this architect");
            }
        }

        public TService GetService<TService>() where TService : class, IService
        {
            var type = typeof(TService);
            if (m_LookupServices.ContainsKey(type))
            {
                return m_LookupServices[type] as TService;
            }
            return Service.Null as TService;
        }

        public void Register(IService service)
        {
            var type = service.GetType();
            if (!m_LookupServices.ContainsKey(type))
            {
                m_LookupServices.Add(type, service);
                m_Services.Add(service);
                service.OnInit();
            }
            else
            {
                Logger.LogError("Service {" + type + "} already exist in this architect");
            }
        }

        public void StartServices()
        {
            Logger.Log("Start All Services", Color.green);
            foreach (var service in m_Services)
            {
                service.OnStart();
            }
        }

        public void DisposeService()
        {
            Logger.Log("Dispose All Services", Color.cyan);
            foreach (var service in m_Services)
            {
                service.OnDispose();
            }
        }
        private void Update()
        {
            foreach (var service in m_Services)
            {
                service.OnUpdate();
            }
        }
    }
}