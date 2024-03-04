using System;

namespace Core
{
    public interface IEventMgr
    {
        int EventHandlerCount { get; }

        int EventCount { get; }

        bool Check<T>(EventHandler<IEventArgs> handler) where T : IEventArgs;

        void Subscribe<T>(EventHandler<IEventArgs> handler) where T : IEventArgs;

        void Unsubscribe<T>(EventHandler<IEventArgs> handler) where T : IEventArgs;

        void Fire(object sender, IEventArgs e);

        void FireNow(object sender, IEventArgs e);

        void FireNow<T>(object sender) where T : IEventArgs;
    }
}