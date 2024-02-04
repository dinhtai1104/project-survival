using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEngine : MonoBehaviour, IStatusEngine
{
    [Serializable]
    private class StatusData
    {
        public EStatus type;
        public ActorBase actorSource;
        public object sourceCast;
        ~StatusData()
        {
            sourceCast = null;
        }
        public void Dispose()
        {
            sourceCast = null;
        }
    }

    private ActorBase _character;
    [SerializeField]
    private List<BaseStatus> _listStatuses = new List<BaseStatus>();

    // For marked status has in storage or not?
    [SerializeField]
    private List<StatusData> _statusData = new List<StatusData>();
    public bool Locked { get; set; }
    public void Initialize(ActorBase character)
    {
        this._character = character;
    }

    public async UniTask<BaseStatus> AddStatus(ActorBase source, EStatus eStatus, object sourceCast)
    {
        if (_character.IsDead() || _character.Tagger.HasTag(ETag.Immune)) return null;
        if (HasStatus(source, eStatus, sourceCast)) return null;
        _statusData.Add(new StatusData { type = eStatus, actorSource = source, sourceCast = sourceCast });
        BaseStatus status = await GetStatus(eStatus);
        status.SetSourceCast(sourceCast);
        _listStatuses.Add(status);
        status.Init(source, _character);
        return status;
    }

    private bool HasStatus(ActorBase source, EStatus eStatus, object sourceCast)
    {
        foreach (var status in _statusData)
        {
            if (status.type == eStatus && status.actorSource == source && status.sourceCast == sourceCast)
            {
                return true;
            }
        }
        return false;
    }

    private async UniTask<BaseStatus> GetStatus(EStatus eStatus)
    {
        var statusRefer = await ResourcesLoader.Instance.LoadAsync<GameObject>("Status/" + eStatus.ToString() + ".prefab");
        var status = PoolManager.Instance.Spawn(statusRefer, transform).GetComponent<BaseStatus>();
        return status;
    }

    public void ClearAllStatuses()
    {
        for (int i = 0; i < _listStatuses.Count; i++)
        {
            _listStatuses[i].Stop();
        }
        _statusData.Clear();
        _listStatuses.Clear();
    }

    public void ClearStatus<TStatus>() where TStatus : BaseStatus
    {
        var type = typeof(TStatus);
        foreach (var status in _listStatuses)
        {
            if (status.GetType() == type)
            {
                status.Stop();
                _statusData.RemoveAll(data =>
                {
                    bool check = data.type == status.Status;
                    if (check)
                    {
                        data.Dispose();
                    }
                    return data.type == status.Status;
                });
            }
        }
    }

    public bool HasStatus<TStatus>() where TStatus : BaseStatus
    {
        var type = typeof(TStatus);
        foreach (var status in _listStatuses)
        {
            if (status.GetType() == type)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsImmnune<TStatus>() where TStatus : BaseStatus
    {
        var type = typeof(TStatus);
        foreach (var status in _listStatuses)
        {
            if (status.GetType() == type)
            {
                return true;
            }
        }
        return false;
    }

    public void SetImmune<TStatus>(bool immune) where TStatus : BaseStatus
    {
    }

    public void Tick(float dt)
    {
        for (int i = _listStatuses.Count - 1; i >= 0; i--)
        {
            if (i >= 0 && i <= _listStatuses.Count - 1)
            {
                var status = _listStatuses[i];
                if (status != null)
                {
                    status.Ticks(dt);
                }
            }
        }
    }

    public void LateTick(float dt)
    {
        for (int i = _listStatuses.Count - 1; i >= 0; i--)
        {
            var status = _listStatuses[i];
            if (status == null)
            {
                _listStatuses.RemoveAt(i);
                continue;
            }
            if (!status.IsExpired) continue;
            RemoveStatus(status);
            status.Stop();
            _listStatuses.RemoveAt(i);
        }
    }

    private void RemoveStatus(BaseStatus status)
    {
        _statusData.RemoveAll(data => data.type == status.Status && data.actorSource == status.Source && data.sourceCast == status.SourceCast);
    }
}
