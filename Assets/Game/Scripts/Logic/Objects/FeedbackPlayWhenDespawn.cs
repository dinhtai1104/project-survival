using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackPlayWhenDespawn : MonoBehaviour, IBeforeDestroyObject
{
    public MMF_Player feedback;
    
    public void Action(Collider2D collision)
    {
        feedback.PlayFeedbacks();
    }
}
