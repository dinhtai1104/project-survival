using UnityEngine;

public class HitActionPlaySound : MonoBehaviour, IHitTriggerAction
{
    public AudioClip audioClip;
    public void Action(Collider2D collider)
    {
        Sound.Controller.Instance.PlayOneShot(audioClip);
    }
}