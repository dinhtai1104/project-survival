using UnityEngine;

public class ElipseObject : MonoBehaviour, IGetPositionByAngle
{
    [SerializeField] private float sizeX = 1;
    [SerializeField] private float sizeY = 2;

    public Vector3 GetPosition(float angle)
    {
        var center = (Vector2)transform.position;
        var lastPos = new Vector2(center.x + (sizeX * Mathf.Sin(Mathf.Deg2Rad * angle)),
                            center.y + (sizeY * Mathf.Cos(Mathf.Deg2Rad * angle)));
        return lastPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var center = (Vector2)transform.position;
        float time = 0;

        var lastPos = new Vector2(0f + (10f * Mathf.Sin(Mathf.Deg2Rad * time)),
                            0f + (5f * Mathf.Cos(Mathf.Deg2Rad * time)));
        for (time = 0; time <= 360; time += 1)
        {
            var currentPos = new Vector2(center.x + (sizeX * Mathf.Sin(Mathf.Deg2Rad * time)),
                            center.y + (sizeY * Mathf.Cos(Mathf.Deg2Rad * time)));
            Gizmos.DrawLine(lastPos, currentPos);
            lastPos = currentPos;
        }
    }
}