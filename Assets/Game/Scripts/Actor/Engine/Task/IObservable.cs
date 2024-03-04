using System;

namespace Engine
{
    public interface IObservable<T> where T : IObserver
    {
        void Subscribe(T observer);
        void Unsubscribe(T observer);
    }
}