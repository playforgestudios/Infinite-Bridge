using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneMoreChangeUI : MonoBehaviour
{
    public Image progress;
    public RectTransform btnOneMoreChange;

    public float timeout;
    [SerializeField] private TextMeshProUGUI adNotAvailableText;
    [SerializeField] private GameObject heart;

    // Use this for initialization
    void Awake()
    {
        GameManager.OnEvent += GameManager_OnEvent;
    }

    void StartTimer()
    {
        LeanTween.value(gameObject, updateProgress, 1, 0, timeout).setOnComplete(OnTimeout);
        LeanTween.cancel(btnOneMoreChange);
        btnOneMoreChange.localScale = Vector3.one;
        LeanTween.scale(btnOneMoreChange, new Vector3(1.2f, 1.2f, 1.2f), .5f).setEaseInBounce().setLoopPingPong();
    }

    void updateProgress(float val)
    {
        progress.fillAmount = val;
    }

    void OnTimeout()
    {
        GameManager.Instance.FinishGame();
    }

    public void OnHeartClicked()
    {
        LeanTween.cancel(gameObject);
        GameManager.Instance.PublishEvent("show_incentivized_ad", "omc");
    }

    public void OnCancelClicked()
    {
        LeanTween.cancel(gameObject);
        GameManager.Instance.FinishGame();
    }

    void GameManager_OnEvent(string name, object value)
    {
        if (name == "gamestate")
        {

            if (value.ToString() == "show_omc")
            {
                gameObject.SetActive(true);
                StartTimer();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        // else if (name == "start_omc")
        // {
        //     StartTimer();
        // }
        else if (name == "incentivized_video_completed" && value.ToString() == "omc")
        {
            Debug.Log("One more chance successfull");
            GameManager.Instance.ContinueGame();
        }
        else if (name == "incentivized_video_failed")
        {
            Debug.Log("One more chance failed");
            heart.SetActive(false);
            adNotAvailableText.gameObject.SetActive(true);
            //GameManager.Instance.FinishGame();
        }
    }

    void ShowChanceUI()
    {
        // TODO:
        // if hasKeys, show "keys only" ui
        // else show continue with ad ui
            // allow ads ui only for "GameManager.Instance.MaxChancesWithAd" times
    }

    private void OnDisable()
    {
        heart.SetActive(true);
        adNotAvailableText.gameObject.SetActive(false);
    }
}