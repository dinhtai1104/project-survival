using Engine;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using ExtensionKit;

public class PlayAnimationTask : Task
{
    [SerializeField] private SkeletonAnimation m_Skeleton;

    [SerializeField, SpineAnimation(dataField = "m_Skeleton")]
    private string m_Animation;

    [SerializeField, SpineEvent(dataField = "m_Skeleton")]
    private string m_EventName;

    [SerializeField] private bool m_Loop;
    [SerializeField] private bool m_WaitingForCompletion = true;
    [SerializeField] private UnityEvent m_OnEventTrigger;

    private EventData m_EventData;

    protected override void Awake()
    {
        base.Awake();
        if (m_EventName.IsNotNull())
        {
            m_EventData = m_Skeleton.Skeleton.Data.FindEvent(m_EventName);
            m_Skeleton.AnimationState.Event += OnEvent;
        }
    }

    public override void Begin()
    {
        base.Begin();
        if (!m_WaitingForCompletion)
        {
            m_Skeleton.ChangeAnimation(m_Animation, m_Loop);
            IsCompleted = true;
        }
    }

    public override void Run()
    {
        base.Run();
        if (m_Skeleton.EnsureSetAnimation(m_Animation, m_Loop))
        {
            if (m_WaitingForCompletion)
            {
                if (m_Skeleton.IsCurrentAnimationComplete())
                {
                    IsCompleted = true;
                }
            }
        }
    }

    private void OnEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (!IsRunning || m_EventData != e.Data || !m_Skeleton.IsPlaying(m_Animation)) return;
        IsCompleted = true;
        m_OnEventTrigger?.Invoke();
    }
}