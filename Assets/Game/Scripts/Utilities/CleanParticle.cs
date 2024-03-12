using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CleanParticle : MonoBehaviour
{
    [SerializeField] private bool m_CleanOnEnable = true;
    [SerializeField, ReadOnly] private ParticleSystem[] m_Particles;
    [SerializeField, ReadOnly] private TrailRenderer[] m_Trails;

    private void Awake()
    {
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Trails = GetComponentsInChildren<TrailRenderer>();
    }

    private void Start()
    {
        foreach (var m_Particle in m_Particles)
        {
            m_Particle.Clear();
            m_Particle.Simulate(0, true, true);
        }
        foreach (var trail in m_Trails)
        {
            trail.Clear();
        }
    }

    private void OnEnable()
    {
        foreach (var m_Particle in m_Particles)
        {
            m_Particle.Clear();
            m_Particle.Simulate(0, true, true);
        }
        foreach (var trail in m_Trails)
        {
            trail.Clear();
        }
    }

    private void OnDisable()
    {
        foreach (var m_Particle in m_Particles)
        {
            m_Particle.Clear();
            m_Particle.Simulate(0, true, true);
        }
        foreach (var trail in m_Trails)
        {
            trail.Clear();
        }
    }
}