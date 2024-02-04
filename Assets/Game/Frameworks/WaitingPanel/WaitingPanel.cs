using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanel:MonoBehaviour
{
    private static UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> op;
    public static void Create(System.Action<WaitingPanel> onDone)
    {
        if (Instance != null)
        {
            onDone?.Invoke(Instance);
            return;
        }
        op = UnityEngine.AddressableAssets.Addressables.InstantiateAsync("WaitingCanvas", null);
        op.Completed += (op) =>
        {
            Instance = op.Result.GetComponent<WaitingPanel>();
            onDone?.Invoke(Instance);
        };
    }

    public static void Show(System.Action onShown=null,float backGroundAlpha=0.8f)
    {
        Create(panel =>
        {
            panel.SetUp(backGroundAlpha);
            onShown?.Invoke();
        });
    }
    public static async void Hide()
    {
        await op.ToUniTask();
        Instance.Deactive();
    }


    public static WaitingPanel Instance;
    [SerializeField]
    private GameObject closeBtn;
    [SerializeField]
    private Image backGroundImg;
  

    public void SetUp(float backGroundAlpha = 0.8f)
    {
        Color c = backGroundImg.color;
        c.a = backGroundAlpha;
        backGroundImg.color = c;
        Active();

        closeBtn.SetActive(false);
        Invoke(nameof(ShowClose), 2);
    }
    public void ShowClose()
    {
        closeBtn.SetActive(true);
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }
    public  void Deactive()
    {
        gameObject.SetActive(false);
    }
}
