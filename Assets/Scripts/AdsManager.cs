using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
#if UNITY_ANDROID
    private string _bannerId = "ca-app-pub-5080271606964191/6696381304";
    private string _interstitialId = "ca-app-pub-5080271606964191/8466410938";
    private string _rewardedId = "ca-app-pub-5080271606964191/4379255441";
    private string _rewardedInterId = "ca-app-pub-5080271606964191/7797751544";
#else
  private string _adUnitId = "unused";
#endif

    private BannerView _bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    [SerializeField] private int interstitialAdCountDown = 3;

    void Awake()
    {
        GameManager.OnEvent += GameManager_OnEvent;
    }
    
    public void Start()
    {
        var isFirstLaunch = PlayerPrefs.GetInt("isFirstLaunch", 1);
        if (isFirstLaunch == 1)
            return;
        InitializeAds();
    }

    void InitializeAds()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((initStatus) =>
        {
            ChooseContentRating();
            StartCoroutine(LoadAdsAfterInitialize());
        });
    }

    void ChooseContentRating()
    {
        RequestConfiguration requestConfiguration = MobileAds.GetRequestConfiguration();
        switch (GameManager.PlayerAge)
        {
            case <=13:
                requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
                requestConfiguration.MaxAdContentRating = MaxAdContentRating.G;
                break;
            case <= 17:
                requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
                requestConfiguration.MaxAdContentRating = MaxAdContentRating.T;
                break;
            case >= 22:
                requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.False;
                requestConfiguration.MaxAdContentRating = MaxAdContentRating.MA;
                break;
            default:
                requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
                requestConfiguration.MaxAdContentRating = MaxAdContentRating.Unspecified;
                break;
        }
        
        MobileAds.SetRequestConfiguration(requestConfiguration);
    }

    IEnumerator LoadAdsAfterInitialize()
    {
        LoadBannerAd();
        yield return new WaitForSeconds(0.2f);
        //StartCoroutine(LoadRewardedInterstitialAd());
        StartCoroutine(LoadRewardedAd());
        StartCoroutine(LoadInterstitialAd());
    }

    public void CreateBannerView()
    {
        string _adUnitId = _bannerId;
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
    }

    private void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Game");

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private IEnumerator LoadInterstitialAd()
    {
        string _adUnitId = _interstitialId;
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Game");

        // send the request to load the ad.
        bool done = false;
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    done = true;
                    return;
                }
                done = true;

                interstitialAd = ad;
            });
        yield return new WaitUntil(() => done);
    }

    public void ShowInterAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            print("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            print("Interstitial ad is not ready yet.");
        }
        RegisterEventHandlers(interstitialAd);
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        if (ad == null) { StartCoroutine(LoadInterstitialAd()); return; }

        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            StartCoroutine(LoadInterstitialAd());
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            StartCoroutine(LoadInterstitialAd());
        };
    }

    public IEnumerator LoadRewardedAd()
    {
        string _adUnitId = _rewardedId;
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Game");

        bool done = false;
        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                done = true;
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    return;
                }
                done = true;
                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
        yield return new WaitUntil(() => done);
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
        RegisterEventHandlers(rewardedAd);
    }
    
    private void RegisterEventHandlers(RewardedAd ad)
    {
        if (ad == null)
        {
            GameManager.Instance.PublishEvent("incentivized_video_failed");
            StartCoroutine(LoadRewardedAd());
            return;
        }
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            GameManager.Instance.PublishEvent("incentivized_video_completed", "omc");
            StartCoroutine(LoadRewardedAd());
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            GameManager.Instance.PublishEvent("incentivized_video_failed");
            StartCoroutine(LoadRewardedAd());
        };
    }
    
    public IEnumerator LoadRewardedInterstitialAd()
    {
        string _adUnitId = _rewardedInterId;
        // Clean up the old ad before loading a new one.
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        bool done = false;
        // send the request to load the ad.
        RewardedInterstitialAd.Load(_adUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                done = true;
                if (error != null || ad == null)
                {
                    return;
                }
                done = true;
                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedInterstitialAd = ad;
            });
        yield return new WaitUntil(() => done);
    }
    
    public void ShowRewardedInterstitialAd()
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
        RegisterReloadHandler(rewardedInterstitialAd);
    }
    
    private void RegisterReloadHandler(RewardedInterstitialAd ad)
    {
        if (ad == null)
        {
            StartCoroutine(LoadRewardedInterstitialAd());
            return;
        }
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            // Reload the ad so that we can show another as soon as possible.
            
            StartCoroutine(LoadRewardedInterstitialAd());
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Reload the ad so that we can show another as soon as possible.
            
            StartCoroutine(LoadRewardedInterstitialAd());
        };
    }
    
    void GameManager_OnEvent(string name, object value)
    {
        if (name == "show_incentivized_ad")
        {
            ShowRewardedAd();
        }
        else if (name == "Initialize Ads")
        {
            InitializeAds();
        }
        else if (name == "show_interstitial_ad")
        {
            interstitialAdCountDown--;
            if (interstitialAdCountDown <= 0)
            {
                ShowInterAd();
                interstitialAdCountDown = 3;
            }
        }
    }

    void OnDestroy()
    {
        GameManager.OnEvent -= GameManager_OnEvent;
    }
}