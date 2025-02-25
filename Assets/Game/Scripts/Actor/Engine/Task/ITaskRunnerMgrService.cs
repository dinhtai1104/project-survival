using com.sparkle.core;

namespace Engine
{
    public interface ITaskRunnerMgrService : IService, IInitializable, IUpdatable
    {
        void Subscribe(TaskRunner observer);
        void Unsubscribe(TaskRunner observer);
    }
}