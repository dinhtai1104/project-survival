using Cysharp.Threading.Tasks;
using UnityEngine;

public class UITweenPlayAudio : UITweenBase
{
    public string SFX_Address = "SFX/UI/";
    [Range(0, 1)]
    public float volumn = 1;
    public override async UniTask Show()
    {
        var audio = await ResourcesLoader.Instance.LoadAsync<AudioClip>(SFX_Address);
        Sound.Controller.Instance.PlayOneShot(audio, volumn);
    }

    public override async UniTask Hide()
    {
        await UniTask.Yield(cancellationToken: cancelToken.Token);
        ResourcesLoader.Instance.UnloadAsset<AudioClip>(SFX_Address);
    }
}
