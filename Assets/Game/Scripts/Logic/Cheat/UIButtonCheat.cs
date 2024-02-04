using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonCheat : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI cheatName;

    public void SetCheat(string name, System.Action cb)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { cb?.Invoke(); });
        cheatName.text = name;
    }
}