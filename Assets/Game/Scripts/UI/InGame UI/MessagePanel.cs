using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanel : UI.Panel
{
    private static UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> op;
    public static void Create(System.Action<UI.Panel> onDone)
    {
        if (Instance != null)
        {
            onDone?.Invoke(Instance);
            return;
        }
        UnityEngine.AddressableAssets.Addressables.InstantiateAsync("MessagePanel", UI.PanelManager.Instance.transform).Completed += (op) =>
        {
            MessagePanel.op = op;
            Instance = op.Result.GetComponent<MessagePanel>();
            Instance.PostInit();
            onDone?.Invoke(Instance);
        };
    }
    private void OnDisable()
    {
        if (op.IsValid())
        {
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(op);
            Instance = null;
        }
    }
    public static MessagePanel Instance;
    public TMPro.TextMeshProUGUI messageText;
    public override void PostInit()
    {
        Instance = this;
    }
    public void SetUp(string message)
    {
        messageText.text = message;
        Show();
    }
    System.Action onClose;
    public void SetUp(string message,System.Action onClose)
    {
        this.onClose = onClose;
        messageText.text = message;
        Show();
    }
    public override void Deactive()
    {
        base.Deactive();
        onClose?.Invoke();
    }
}
