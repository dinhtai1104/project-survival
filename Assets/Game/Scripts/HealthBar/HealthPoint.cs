using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    [SerializeField]
    private Image barImg;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
    public void Set(float fill)
    {
        barImg.fillAmount = fill;
    }
}
