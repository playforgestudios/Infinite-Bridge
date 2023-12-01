using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

public class GPGSManager : MonoBehaviour
{
    internal static bool IsAuthenticated;
    string Username;
    string UserId;
    string AuthToken;
    string AuthenticationFailureMessage;

    [SerializeField] Button SignInBtn;
    public static GPGSManager Instance;

    private void Awake()
    {
        //Instance = this;
    }

    // public void Start()
    // {
    //     SignInBtn.gameObject.SetActive(false);
    //     PlayGamesPlatform.Activate();
    //     StartCoroutine(SignInOnStart());
    // }

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
        AuthToken = isAuthenticated ? Social.localUser.id : null;
    }

    public void SignInOnBtn()
    {
        // if (!IsAuthenticated)
        // {
        //     PlayGamesPlatform.Instance.ManuallyAuthenticate(authResult =>
        //     {
        //         if(authResult == SignInStatus.Success)
        //         {
        //             PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) => SetAuthInfo(true));
        //             Debug.Log("Signed in!");
        //         }
        //         else
        //         {
        //             AuthenticationFailureMessage = authResult.ToString();
        //             SetAuthInfo(false);
        //             Debug.Log(AuthenticationFailureMessage);
        //         }
        //     });
        // }
    }

}
