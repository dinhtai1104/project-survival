using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GenObjectByAngle : MonoBehaviour
{
    [SerializeField] private IGetPositionByAngle Position;

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
        Position = GetComponent<IGetPositionByAngle>();
        for (int i = 0; i < amount; i++)
        {
            var lzer = Instantiate(prefab, transform);
            lzer.gameObject.SetActive(true);
            lzer.transform.position = Position.GetPosition(angle);
            angle += angleIncre;
            list.Add(lzer);
        }
    }

}