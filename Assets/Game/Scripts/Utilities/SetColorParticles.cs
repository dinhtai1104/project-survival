using UnityEngine;

public class SetColorParticles : MonoBehaviour
{
    private ParticleSystem[] m_Particles;
    public void SetColor(Color c)
    {
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in m_Particles)
        {
            c.a = 1;
            SetTargetColor(particle, c);
        }
    }
    public virtual void SetTargetColor(ParticleSystem t, Color targetColor)
    {
        var main = t.colorOverLifetime;
        var gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(targetColor, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
        main.color = gradient;

        ParticleSystem.MainModule main1 = t.main;
        main1.startColor = targetColor;

        ParticleSystem.MainModule main2 = t.main;
        main2.startColor = targetColor;
    }
}