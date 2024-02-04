using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : Task
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AssetReferenceT<AudioClip> audioReferences;
    [SerializeField] private bool isLoop;

    private void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override async UniTask Begin()
    {
        await base.Begin();
        var task = await audioReferences.LoadAssetAsync().Task;
        Sound.Controller.Instance.Play(audioSource, task, isLoop);
        IsCompleted = true;
    }
}