using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using Google.Play.Common;

public class RateUs : MonoBehaviour
{
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;
    PlayReviewInfo _playReviewInfo;
    void Start()
    {
        var timesGameLaunched = PlayerPrefs.GetInt("timesGameLaunched", 0);
        timesGameLaunched++;
        PlayerPrefs.SetInt("timesGameLaunched", timesGameLaunched);
        if(timesGameLaunched == 5 || timesGameLaunched == 10 || timesGameLaunched == 15)
        {
            StartCoroutine(RequestInfo());
        }
        
    }

    IEnumerator RequestInfo()
    {
        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
        
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
    }
}
