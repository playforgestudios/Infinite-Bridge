using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using TMPro;

public class GPGSManager : MonoBehaviour
{
    internal static bool IsAuthenticated;
    string Username;
    string UserId;
    string AuthToken;
    string AuthenticationFailureMessage;

    [SerializeField] Button SignInBtn;
    [SerializeField] TextMeshProUGUI WelcomeUserMessage;
    [SerializeField] TextMeshProUGUI StatusText;

    public void Start()
    {
        SignInBtn.gameObject.SetActive(false);
        PlayGamesPlatform.Activate();
        StartCoroutine(SignInOnStart());
    }

    IEnumerator SignInOnStart()
    {
        bool done = false;
        PlayGamesPlatform.Instance.Authenticate((authResult) =>
        {
            if(authResult == SignInStatus.Success)
            {
                done = true;
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) => SetAuthInfo(true));
                Debug.Log("Signed in!");
            }
            else
            {
                done = true;
                AuthenticationFailureMessage = authResult.ToString();
                SetAuthInfo(false);
                Debug.Log(AuthenticationFailureMessage);
            }
        });
        yield return new WaitUntil(() => done);
    }

    void SetAuthInfo(bool isAuthenticated)
    {
        SignInBtn.gameObject.SetActive(!isAuthenticated);

        IsAuthenticated = isAuthenticated;
        Username = isAuthenticated ? Social.localUser.userName : "Guest";
        StatusText.text = isAuthenticated ? "Authenticated" : "Not Authenticated: "+ AuthenticationFailureMessage;
        AuthToken = isAuthenticated ? Social.localUser.id : null;
        WelcomeUserMessage.text = "Welcome, " + Username;
    }

    public void SignInOnBtn()
    {
        if (!IsAuthenticated)
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(authResult =>
            {
                if(authResult == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) => SetAuthInfo(true));
                    Debug.Log("Signed in!");
                }
                else
                {
                    AuthenticationFailureMessage = authResult.ToString();
                    SetAuthInfo(false);
                    Debug.Log(AuthenticationFailureMessage);
                }
            });
        }
    }

}
