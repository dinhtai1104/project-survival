using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BaseEventArgs<T> : IEventArgs where T : IEventArgs
    {
        public int Id
        {
            get { return typeof(T).GetHashCode(); }
        }
    }
}