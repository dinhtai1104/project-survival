using com.sparkle.core;
using Cysharp.Threading.Tasks;

[Service(typeof(DataManager))]
public class DataManager : LiveSingleton<DataManager>, IService, IInitializable
{
    public static DatasaveManager Save;
    public static DatabaseManager Base;
    public static DataliveManager Live;

    public bool Initialized { set; get; }

    public int Priority => 0;

    public UniTask OnInitialize(IArchitecture architecture)
    {
        DataliveManager.Instance.Init(transform);
        DatabaseManager.Instance.Init(transform);
        DatasaveManager.Instance.Init(transform);
        Initialized = true;
        return UniTask.CompletedTask;
    }
}
