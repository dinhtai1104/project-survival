using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelUI : MonoBehaviour
{
    [SerializeField]
    private Image Bar;

    Transform target,_transform;
    Vector3 offset;

    [SerializeField]
    private GameObject main; 
    [SerializeField]
    PanelFadeAnimation anim;
    public void SetUp(Transform target,Vector3 offset)
    {
        _transform = transform;
        this.offset = offset;
        this.target = target;
        gameObject.SetActive(true);
        SetActive(false);
    }
    public void Show()
    {
        if (main.gameObject.activeSelf) return;
        anim.Show();
    }
    public void Hide()
    {
        SetActive(false);
    }
    public void SetActive(bool active)
    {

        main.SetActive(active);
    }
    public void UpdateFuel(float current,float max)
    {
        Bar.fillAmount = current / max;
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        _transform.localPosition = target.position + offset;
    }
}
