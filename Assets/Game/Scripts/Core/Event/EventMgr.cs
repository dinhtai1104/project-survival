using com.sparkle.core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [Service(typeof(IEventMgrService))]
    public class EventMgr : MonoBehaviour, IEventMgrService
    {
        private readonly Dictionary<int, EventHandler<IEventArgs>> m_EventHandlers =
            new Dictionary<int, EventHandler<IEventArgs>>();

        private readonly Queue<Event> m_Events = new Queue<Event>();

        public int EventHandlerCount
        {
            get { return m_EventHandlers.Count; }
        }

        public int EventCount
        {
            get { return m_Events.Count; }
        }

        public int Priority => 1;

        public bool Check<T>(EventHandler<IEventArgs> handler) where T : IEventArgs
        {
            if (handler == null) throw new CoreException("Event handler is invalid.");
            return m_EventHandlers.ContainsKey(typeof(T).GetHashCode());
        }

        public void Subscribe<T>(EventHandler<IEventArgs> handler) where T : IEventArgs
        {
            if (handler == null) throw new CoreException("Event handler is invalid.");
            var id = typeof(T).GetHashCode();
            if (!m_EventHandlers.TryGetValue(id, out var eventHandler))
                m_EventHandlers[id] = handler;
            else
            {
                eventHandler += handler;
                m_EventHandlers[id] = eventHandler;
            }
        }

        public void Unsubscribe<T>(EventHandler<IEventArgs> handler) where T : IEventArgs
        {
            if (handler == null) throw new CoreException("Event handler is invalid.");
            var id = typeof(T).GetHashCode();
            if (m_EventHandlers.TryGetValue(id, out var eventHandler))
            {
                if (eventHandler != null)
                {
                    eventHandler -= handler;
                    if (eventHandler == null)
                    {
                        m_EventHandlers.Remove(id);
                    }
                    else
                    {
                        m_EventHandlers[id] = eventHandler;
                    }
                }
            }
        }

        public void Fire(object sender, IEventArgs e)
        {
            Event @event = (object)e != null
            ? Event.Create(sender, e)
                : throw new CoreException("Event is invalid.");
            lock (m_Events)
            {
                m_Events.Enqueue(@event);
            }
        }

        public void FireNow(object sender, IEventArgs e)
        {
            if ((object)e == null) throw new CoreException("Event is invalid.");
            HandleEvent(sender, e);
        }

        public void FireNow<T>(object sender) where T : IEventArgs
        {
            HandleEvent(sender, typeof(T).GetHashCode());
        }

        public void OnDispose()
        {
            lock (m_Events)
            {
                m_Events.Clear();
            }
        }

        public void OnUpdate()
        {
            lock (m_Events)
            {
                while (m_Events.Count > 0)
                {
                    Event @event = m_Events.Dequeue();
                    HandleEvent(@event.Sender, @event.EventArgs);
                }
            }
        }

        private void HandleEvent(object sender, IEventArgs e)
        {
            if (m_EventHandlers.TryGetValue(e.Id, out var eventHandler))
            {
                eventHandler?.Invoke(sender, e);
            }
        }

        private void HandleEvent(object sender, int id)
        {
            if (m_EventHandlers.TryGetValue(id, out var eventHandler))
            {
                eventHandler?.Invoke(sender, null);
            }
        }

        public UniTask OnInitialize(IArchitecture architecture)
        {
            throw new NotImplementedException();
        }

        private sealed class Event
        {
            private object m_Sender;
            private IEventArgs m_EventArgs;

            private Event()
            {
                m_Sender = (object)null;
                m_EventArgs = default(IEventArgs);
            }

            public object Sender => m_Sender;

            public IEventArgs EventArgs => m_EventArgs;

            public static Event Create(object sender, IEventArgs e)
            {
                Event @event = new Event();
                @event.m_Sender = sender;
                @event.m_EventArgs = e;
                return @event;
            }
        }
    }
}