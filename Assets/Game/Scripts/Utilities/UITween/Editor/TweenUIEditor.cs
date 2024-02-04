using UnityEditor;
using UnityEngine;

public class TweenUIEditor
{
    [MenuItem("GameObject/UITween/Fade Move", priority = -6)]
    public static void CreateTween_FadeMove()
    {
        var go = new GameObject("Tween Fade Move", typeof(UITweenFadeMove));
        go.transform.parent = GetCurrentSelectParent();
        Selection.activeObject = go;
    }
    [MenuItem("GameObject/UITween/Scale", priority = -5)]
    public static void CreateTween_Scale()
    {
        var go = new GameObject("Tween Scale", typeof(UITweenScale));
        go.transform.parent = GetCurrentSelectParent();
        Selection.activeObject = go;
    }
    [MenuItem("GameObject/UITween/Play Particle", priority = -4)]
    public static void CreateTween_PLayParticle()
    {
        var go = new GameObject("Tween Play Particle", typeof(UITweenPlayParticleSystem));
        go.transform.parent = GetCurrentSelectParent();
        Selection.activeObject = go;
    }
    [MenuItem("GameObject/UITween/Play Audio", priority = -3)]
    public static void CreateTween_PlayAudio()
    {
        var go = new GameObject("Tween Play Audio", typeof(UITweenPlayAudio));
        go.transform.parent = GetCurrentSelectParent();
        Selection.activeObject = go;
    }

    public static Transform GetCurrentSelectParent()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeGameObject.transform;
        }
        return null;
    }
}