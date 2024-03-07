using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Engine
{
    public class ShareValueEngine : ISharedEngine
    {
        [ShowInInspector]
        private Dictionary<string, IShareValue> Dictionary = new Dictionary<string, IShareValue>();

        public T GetShared<T>(string key)
        {
            if (Dictionary.ContainsKey(key)) return ((IShareValue<T>)Dictionary[key]).Value;
            return default;
        }

        public void SetShared<T>(string key, T value)
        {
            if (!Dictionary.ContainsKey(key))
            {
                Dictionary.Add(key, new SharedValueBase<T>() { Value = value });
            }
            else
            {
                Dictionary[key] = new SharedValueBase<T>() { Value = value };
            }
        }

        public bool HasShared(string key)
        {
            return Dictionary.ContainsKey(key);
        }
        public void Removed(string key)
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary.Remove(key);
            }
        }
        public void ClearAll()
        {
            Dictionary.Clear();
        }
    }
}