using Cysharp.Threading.Tasks;

#if UNITY_ANDROID
using Google.Play.Review;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewHandler
{
#if UNITY_ANDROID
    private static ReviewManager reviewManager;
    private static PlayReviewInfo playReviewInfo;
#endif

    public static async UniTask<bool> Request()
    {
#if UNITY_ANDROID
        Debug.Log("REQUEST");
        if(reviewManager==null)
            reviewManager = new ReviewManager();

        var requestFlowOperation = reviewManager.RequestReviewFlow();
        await requestFlowOperation;
        Debug.Log("REQUEST "+ requestFlowOperation.Error);

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            return false;
        }
        playReviewInfo = requestFlowOperation.GetResult();
        return await LaunchReview();
#elif UNITY_IOS
        if (UnityEngine.iOS.Device.RequestStoreReview())
        {
            return true;
        }
        else
        {
            return false;
        }
#endif
        return true;
    }
    private static async UniTask<bool> LaunchReview()
    {
#if UNITY_ANDROID
        Debug.Log("LaunchReview");
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        await launchFlowOperation;
        Debug.Log("LaunchReview "+ launchFlowOperation.Error);
        playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            return false;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
#endif

        return true;
    }
}
