using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerUnityEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> m_EventTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        m_EventTrigger?.Invoke(other);
    }
}
