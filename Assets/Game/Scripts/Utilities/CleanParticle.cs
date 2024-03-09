using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CleanParticle : MonoBehaviour
{
    [SerializeField] private bool m_CleanOnEnable = true;
    [SerializeField, ReadOnly] private ParticleSystem[] m_Particles;

    private void Awake()
    {
        if (m_Particles == null)
        {
            m_Particles = GetComponentsInChildren<ParticleSystem>();
        }
    }

    private void Start()
    {
        if (m_CleanOnEnable) return;
        foreach (var m_Particle in m_Particles)
        {
            m_Particle.Clear();
            m_Particle.Simulate(0, true, true);
            m_Particle.Play();
        }
    }

    private void OnEnable()
    {
        if (!m_CleanOnEnable) return;
        foreach (var m_Particle in m_Particles)
        {
            m_Particle.Clear();
            m_Particle.Simulate(0, true, true);
            m_Particle.Play();
        }
    }
}