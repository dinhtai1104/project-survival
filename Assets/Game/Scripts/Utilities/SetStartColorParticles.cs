using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SetStartColorParticles : SetColorParticles
{
    public override void SetTargetColor(ParticleSystem t, Color targetColor)
    {
        var main = t.main;
        var gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(targetColor, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
        //main.color = targetColor;
        var minmax = new MinMaxGradient { color = targetColor, colorMax = targetColor, colorMin = targetColor };
        main.startColor = minmax;
        //ParticleSystem.MainModule main1 = t.main;
        //main1.startColor = targetColor;

        //ParticleSystem.MainModule main2 = t.main;
        //main2.startColor = targetColor;
    }
}