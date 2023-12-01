 using System;
 using System.Collections;
 using UnityEngine;
 using Google.Play.AppUpdate;
 using Google.Play.Common;

 public class InAppUpdateManager : MonoBehaviour
{
    private AppUpdateManager appUpdateManager;
    [SerializeField] private GameObject downloadingBar;
    internal static bool isFlexibleUpdateDownloaded;

    private void Awake()
    {
        GameManager.OnEvent += GameManager_OnEvent;
    }

    void Start()
    {
        downloadingBar.SetActive(false);
        if (Application.platform == RuntimePlatform.Android)
        {
            this.appUpdateManager = new AppUpdateManager();
            StartCoroutine(CheckForUpdate());
        }
    }
    
    IEnumerator CheckForUpdate()
    {
        print("checking for update");
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();
    
        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;
    
        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            var stalenessDays = appUpdateInfoResult.ClientVersionStalenessDays;
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
    
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                    //var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
                    //StartCoroutine(StartFlexibleUpdate(appUpdateInfoResult, appUpdateOptions));

                    if (stalenessDays > 2)
                    {
                        var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                        StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
                    }
            }
            else
            {
                print("App is up to date.");
            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
    }
    
    IEnumerator StartFlexibleUpdate(AppUpdateInfo appUpdateInfoResult, AppUpdateOptions appUpdateOptions)
    {
        if (appUpdateInfoResult != null)
        {
            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = appUpdateManager.StartUpdate(
                appUpdateInfoResult,
                appUpdateOptions);

            //wait until the update request completes
            while (!startUpdateRequest.IsDone)
            {
                
                // For flexible flow,the user can continue to use the app while
                // the update downloads in the background. You can implement a
                // progress bar showing the download status during this time.
                yield return null;
            }

            if (startUpdateRequest.Status != AppUpdateStatus.Failed)
            {
                downloadingBar.SetActive(true);
            }
            
            // keep returning until the app is downloaded
            while (startUpdateRequest.Status != AppUpdateStatus.Downloaded)
            {
                yield return null;
            }

            downloadingBar.SetActive(false);
            isFlexibleUpdateDownloaded = true;
        }
    
    }
    
    IEnumerator CompleteFlexibleUpdate()
    {
        var result = appUpdateManager.CompleteUpdate();
        yield return result;
        if (result.IsDone)
        {
            isFlexibleUpdateDownloaded = false;
            
        }
        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (e.g. by
        // logging result.Error or by displaying a message to the user).
    }
    
    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoResult, AppUpdateOptions appUpdateOptions)
    {
        if (appUpdateInfoResult != null)
        {
            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = appUpdateManager.StartUpdate(
                appUpdateInfoResult,
                appUpdateOptions);
            yield return startUpdateRequest;
    
            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (for
            // example, by logging result.Error or by displaying a message to the user).
        }
    }

    void GameManager_OnEvent(string name, object value)
    {
        if (name == "start installing update")
        {
            StartCoroutine(CompleteFlexibleUpdate());
        }
    }

    void OnDestroy()
    {
        GameManager.OnEvent -= GameManager_OnEvent;
    }
}
