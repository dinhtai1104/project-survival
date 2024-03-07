namespace Engine
{
    public interface ISharedEngine
    {
        void SetShared<T>(string key, T value);
        T GetShared<T>(string key);
        bool HasShared(string key);
        void Removed(string key);
        void ClearAll();
    }
}