using Game.GameActor;
using UnityEngine;

public class ScreenCharacterTag : MonoBehaviour
{
    private Transform _transform;
    public Transform Transform
    {
        get
        {
            if (_transform == null)
            {
                _transform = transform;
            }
            return _transform;
        }
    }
    [SerializeField]
    private float offsetX=1, offsetY=1;
    Transform target;

    float size = 0;
    float screenRatio = 1;
    Transform camera;
    public void SetUp(ActorBase actor)
    {
        size = Camera.main.orthographicSize;
       screenRatio = Screen.width * 1f / Screen.height;
        camera = Camera.main.transform;
        target = actor.GetMidTransform();
       
        SetActive(true);
    }

    private void Update()
    {
        Vector3 camPos = camera.position;
        camPos.z = 0;

        Vector3 direction = target.position - camPos;

        direction.x = Mathf.Clamp(direction.x, -size * screenRatio+offsetX, size * screenRatio-offsetX);
        direction.y = Mathf.Clamp(direction.y, -size+offsetY, size-offsetY );
        Transform.position = camPos + direction;
        Transform.up = target.position-Transform.position;
 
    }

    public void SetActive(bool v)
    {
        gameObject.SetActive(v);
    }
}
