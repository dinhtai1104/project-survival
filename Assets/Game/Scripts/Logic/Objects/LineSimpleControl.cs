using UnityEngine;

public class LineSimpleControl : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;

    [SerializeField] private Transform[] points;

    private void OnEnable()
    {
        //points[0].localPosition = points[1].localPosition = Vector3.zero;
    }

    public void SetPos(int index, Vector3 pos)
    {
        points[index].position = pos;
    }

    public Vector3 GetPos(int index)
    {
        return points[index].position;
    }

    private void LateUpdate()
    {
        lr.SetPosition(0, points[0].localPosition);
        lr.SetPosition(1, points[1].localPosition);
    }
}