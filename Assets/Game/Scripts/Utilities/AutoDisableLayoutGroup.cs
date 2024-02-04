using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AutoDisableLayoutGroup : MonoBehaviour
{
    private async void OnEnable()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.01f));
        GetComponent<LayoutGroup>().enabled = false;
    }
}