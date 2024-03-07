namespace Engine
{
    public class NullSharedEngine : ISharedEngine
    {
        public void ClearAll()
        {
        }

        public T GetShared<T>(string key)
        {
            return default;
        }

        public bool HasShared(string key)
        {
            return false;
        }

        public void Removed(string key)
        {
        }

        public void SetShared<T>(string key, T value)
        {
        }
    }
}