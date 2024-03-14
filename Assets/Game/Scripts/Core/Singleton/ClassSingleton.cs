using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ClassSingleton<T> : IDisposable where T : class
{
    private static T m_Instance;
    public static T Instance => m_Instance;

    public ClassSingleton()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
    }

    ~ClassSingleton()
    {
        m_Instance = null;
    }

    public void Dispose()
    {
        m_Instance = null;
    }
}
