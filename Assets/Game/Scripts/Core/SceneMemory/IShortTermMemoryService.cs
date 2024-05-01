using com.sparkle.core;

public interface IShortTermMemoryService : IService
{
    bool CheckAndForget<T>() where T : ISceneMemory;
    bool Forget<T>() where T : ISceneMemory;
    bool HasMemory<T>() where T : ISceneMemory;
    void Remember<T>(T memory) where T : ISceneMemory;
    T RetrieveMemory<T>() where T : ISceneMemory;
    bool RetrieveMemory<T>(out T memory) where T : ISceneMemory;
}