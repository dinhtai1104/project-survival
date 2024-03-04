using UnityEngine;

public class NotifiableMono : MonoBehaviour, INotifiable
{
    [SerializeField] private NotifyCondition[] m_Conditions;
    [SerializeField] protected GameObject content;

    protected void OnValidate()
    {
        content = transform.Find("Content").gameObject;
        m_Conditions = GetComponentsInParent<NotifyCondition>();
    }


    private void OnEnable()
    {
        if (m_Conditions.Length == 0)
        {
            m_Conditions = GetComponentsInParent<NotifyCondition>();
        }
        content.SetActive(false);
        Register();
        Validate();
    }
    private void OnDisable()
    {
        UnRegister();
    }

    public void Register()
    {
        NotifyManager.Instance.AddNotify(this);
    }

    public void UnRegister()
    {
        NotifyManager.Instance.RemoveNotify(this);
    }

    public void Validate()
    {
        if (m_Conditions == null || m_Conditions.Length == 0)
        {
            return;
        }
        content.SetActive(false);

        try
        {
            foreach (var condition in m_Conditions)
            {
                if (condition == null || !condition.Validate()) continue;
                content.SetActive(true);
                return;
            }
        }
        catch (System.Exception e)
        {

        }
    }
}