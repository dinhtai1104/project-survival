using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;
using Vector2RVO = RVO.Vector2RVO;

public class ObstacleCollect : MonoBehaviour
{
    void Awake()
    {
        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < boxColliders.Length; i++)
        {
            float minX = boxColliders[i].transform.position.x -
                         boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
            float minZ = boxColliders[i].transform.position.z -
                         boxColliders[i].size.z*boxColliders[i].transform.lossyScale.z*0.5f;
            float maxX = boxColliders[i].transform.position.x +
                         boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
            float maxZ = boxColliders[i].transform.position.z +
                         boxColliders[i].size.z*boxColliders[i].transform.lossyScale.z*0.5f;

            IList<Vector2RVO> obstacle = new List<Vector2RVO>();
            obstacle.Add(new Vector2RVO(maxX, maxZ));
            obstacle.Add(new Vector2RVO(minX, maxZ));
            obstacle.Add(new Vector2RVO(minX, minZ));
            obstacle.Add(new Vector2RVO(maxX, minZ));
            Simulator.Instance.addObstacle(obstacle);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}