using Game.GameActor;
using Spine.Unity;
using UnityEngine;

public class ChangeSortingLayerSpineActionState : MonoBehaviour, IStateEnterCallback
{
    public SkeletonAnimation skeleton;
    public string sortingLayer;
    public int orderInLayer;
    public void Action()
    {
        skeleton.GetComponent<MeshRenderer>().sortingLayerName = sortingLayer;
        skeleton.GetComponent<MeshRenderer>().sortingOrder = orderInLayer;
    }

    public void SetActor(ActorBase actor)
    {
    }
}
