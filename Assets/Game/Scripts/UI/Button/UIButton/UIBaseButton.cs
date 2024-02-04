using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public abstract class UIBaseButton : UIEntityBehaviour, IButton, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    protected Button button;
    [SerializeField] private bool isUseOrigin;
    [SerializeField, ShowIf("isUseOrigin", true)] private float originScaleForce = 1;
    protected float originScale = 1;
    public float Origin => isUseOrigin ? originScaleForce : originScale;
    private void Awake()
    {
        originScale = transform.localScale.x;
        originScale = isUseOrigin ? originScaleForce : originScale;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        button = GetComponent<Button>();
    }
#endif

    protected virtual void OnEnable()
    {
        button = GetComponent<Button>();
        button?.onClick.AddListener(Action);
    }
    protected virtual void OnDisable()
    {
        button = GetComponent<Button>();
        button?.onClick.RemoveListener(Action);
    }

    public virtual void OnInit()
    {

    }
    public abstract void Action();
    public virtual void OnHoldEvent() { }
    public virtual void OnReleaseEvent() { }
    public virtual void OnUpdate(float dt) { }
    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
            OnReleaseEvent();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            OnHoldEvent();
    }
}