using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIButtonSound : UIBaseButton
{
    public string SFX_Address = AddressableName.SFX_Button_Common;
    [Range(0, 1)]
    public float volumn = 1;

    public override async void Action()
    {
        Logger.Log("Action Sound: " + gameObject.name);
        var audio = await ResourcesLoader.Instance.LoadAsync<AudioClip>(SFX_Address);
        Sound.Controller.Instance.PlayOneShot(audio, volumn);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
