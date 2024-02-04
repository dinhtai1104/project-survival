using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAffixText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description_OutlineTxt;
    [SerializeField] private TextMeshProUGUI description_NoneOutlineTxt;
    [SerializeField] private Image imageIcon;
    public void SetDescription(string text)
    {
        description_NoneOutlineTxt.text = description_OutlineTxt.text = text;
    }
    public void SetIcon(Sprite sprite)
    {
        imageIcon.sprite = sprite;
    }
    public void SetColor(Color color)
    {
        description_NoneOutlineTxt.color = description_OutlineTxt.color = color;
    }
    public void UseOutline(bool outline)
    {
        description_NoneOutlineTxt.gameObject.SetActive(false);
        description_OutlineTxt.gameObject.SetActive(false);

        if (outline)
        {
            description_OutlineTxt.gameObject.SetActive(true);
        }
        else
        {
            description_NoneOutlineTxt.gameObject.SetActive(true);
        }
    }
}