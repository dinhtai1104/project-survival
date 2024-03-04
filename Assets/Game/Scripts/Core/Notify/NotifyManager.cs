using System.Collections.Generic;
using UnityEngine;

public class NotifyManager : MonoBehaviour
{
    private static NotifyManager instance;
    public static NotifyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NotifyManager>();
            }
            if (instance == null)
            {
                GameObject go = new GameObject("NotifyManager");
                instance = go.AddComponent<NotifyManager>();
            }
            return instance;
        }
    }
    private readonly List<INotifiable> m_AllNotifies = new List<INotifiable>();

    public void AddNotify(INotifiable notify)
    {
        m_AllNotifies.Add(notify);
    }

    public void RemoveNotify(INotifiable notify)
    {
        if (m_AllNotifies.Contains(notify))
        {
            m_AllNotifies.Remove(notify);
        }
    }

    public void ValidateAllNotifies()
    {
        foreach (var notifiable in m_AllNotifies)
        {
            try 
            {
                notifiable.Validate();
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }

    public void Update()
    {
        if (Time.frameCount % 90 == 0)
        {
            ValidateAllNotifies();
        }
    }
}