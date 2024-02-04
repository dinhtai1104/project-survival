using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class TestGenObjectCircle : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int amount;
    [SerializeField] private float radius;
    [SerializeField, ReadOnly] private List<GameObject> list = new List<GameObject>();

    [Button]
    public void TestGen()
    {
        if (list.Count > 0)
        {
            foreach (var go in list)
            {
                if (go != null)
                {
                    DestroyImmediate(go);
                }
            }
            list.Clear();
        }

        float angle = 0;
        float angleIncre = 360f / amount;
        for (int i = 0; i < amount; i++)
        {
            var lzer = Instantiate(prefab, transform);
            lzer.gameObject.SetActive(true);
            lzer.transform.localPosition = Vector3.zero;
            lzer.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, angle);
            lzer.transform.position = GetComponent<IGetPositionByAngle>().GetPosition(angle);
            angle += angleIncre;
            list.Add(lzer);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius / transform.localScale.x);
    }
}