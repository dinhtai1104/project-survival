using TMPro;
using UnityEngine;

public class UIValueChanged : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI oldValue;
    [SerializeField] private TextMeshProUGUI newValue;

    public void SetValue(string label, string oldValue, string newValue)
    {
        this.label.SetText(label);
        this.oldValue.SetText(oldValue);
        this.newValue.SetText(newValue);
    }
}
