using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class GoogleAdMobController : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private float deltaTime;
    private float fps;

    public bool showFpsMeter = true;

    public Text statusText;
    public Text fpsMeter;

    #region UNITY MONOBEHAVIOR METHODS
    public void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        List<string> deviceIds = new List<string>() { AdRequest.TestDeviceSimulator };

        // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
        deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#endif //UNITY_IPHONE

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            statusText.text = "Initialization complete";
            RequestBannerAd();
        });
    }

    private void Update()
    {
        if(showFpsMeter)
        {
            fpsMeter.gameObject.SetActive(true);
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
            fpsMeter.text = string.Format("{0:0.} fps", fps);
        }
        else
        {
            fpsMeter.gameObject.SetActive(false);
        }
    }
    #endregion UNITY MONOBEHAVIOR METHODS

    #region HELPER METHODS
    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("unity-admob-sample")
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }
    #endregion HELPER METHODS

    #region BANNER ADS
    private void RequestBannerAd()
    {
        statusText.text = "Requesting Banner Ad.";

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif //UNITY_EDITOR

        // Clean up banner before reusing
        DestroyBannerAd();

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

        // Add Event Handlers
        bannerView.OnAdLoaded += OnAdLoadedEvent;
        bannerView.OnAdFailedToLoad += OnAdFailedToLoadEvent;
        bannerView.OnAdOpening += OnAdOpeningEvent;
        bannerView.OnAdClosed += OnAdClosedEvent;
        bannerView.OnAdLeavingApplication += OnAdLeavingApplicationEvent;

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            statusText.text = "ad destroied";
        }
    }
    #endregion BANNER ADS

    #region INTERSTITIAL ADS
    public void RequestAndLoadInterstitialAd()
    {
        statusText.text = "Requesting Interstitial Ad.";

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif //UNITY_EDITOR

        // Clean up interstitial before using it
        DestroyInterstitialAd();

        interstitialAd = new InterstitialAd(adUnitId);

        // Add Event Handlers
        interstitialAd.OnAdLoaded += OnAdLoadedEvent;
        interstitialAd.OnAdFailedToLoad += OnAdFailedToLoadEvent;
        interstitialAd.OnAdOpening += OnAdOpeningEvent;
        interstitialAd.OnAdClosed += OnAdClosedEvent;
        interstitialAd.OnAdLeavingApplication += OnAdLeavingApplicationEvent;

        // Load an intersitial ad
        interstitialAd.LoadAd(CreateAdRequest());
    }

    public void ShowInterstitialAd()
    {
        if(interstitialAd != null)
        {
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
            }
            else
            {
                statusText.text = "Interstitial ad is not ready yet";
            }
        }
        else
        {
            statusText.text = "Interstitial ad is not ready yet";
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            statusText.text = "ad destroied";
        }
    }
    #endregion INTERSTITIAL ADS

    #region REWARDED ADS
    public void RequestAndLoadRewardedAd()
    {
        statusText.text = "Requesting Rewarded Ad.";

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif //UNITY_EDITOR

        // create new rewarded ad instance
        rewardedAd = new RewardedAd(adUnitId);

        // Add Event Handlers
        rewardedAd.OnAdLoaded += OnAdLoadedEvent;
        rewardedAd.OnAdFailedToLoad += OnRewardAdFailedToLoadEvent;
        rewardedAd.OnAdOpening += OnAdOpeningEvent;
        rewardedAd.OnAdFailedToShow += OnAdFailedToShowEvent;
        rewardedAd.OnAdClosed += OnAdClosedEvent;
        rewardedAd.OnUserEarnedReward += OnUserEarnedRewardEvent;

        // Create empty ad request
        rewardedAd.LoadAd(CreateAdRequest());
    }

    public void ShowRewardedAd()
    {
        if(rewardedAd != null)
        {
            if(rewardedAd.IsLoaded())
            {
                rewardedAd.Show();
            }
            else
            {
                statusText.text = "Rewarded ad is not ready yet.";
            }
        }
        else
        {
            statusText.text = "Rewarded ad is not ready yet.";
        }
    }
    #endregion REWARDED ADS

    #region EVENT HANDLERS
    public void OnAdLoadedEvent(object sender, EventArgs args)
    {
        statusText.text = "Adloaded event received";
    }

    public void OnAdFailedToLoadEvent(object sender, AdFailedToLoadEventArgs args)
    {
        statusText.text = string.Format("Failed to ReceivedAd event received with message : {0}", args.Message);
    }

    public void OnRewardAdFailedToLoadEvent(object sender, AdErrorEventArgs args)
    {
        statusText.text = string.Format("Failed to RewardAd event received with message : {0}", args.Message);
    }

    public void OnAdOpeningEvent(object sender, EventArgs args)
    {
        statusText.text = "AdOpened event received";
    }

    public void OnAdFailedToShowEvent(object sender, AdErrorEventArgs args)
    {
        statusText.text = string.Format("Failed to Show event received with message : {0}", args.Message);
    }

    public void OnUserEarnedRewardEvent(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        statusText.text = string.Format("RewardAdRewarded event received for {0} {1}", amount.ToString(), type);
    }

    public void OnAdClosedEvent(object sender, EventArgs args)
    {
        statusText.text = "AdClosed event received";
    }

    public void OnAdLeavingApplicationEvent(object sender, EventArgs args)
    {
        statusText.text = "AdLeavingApplication event received";
    }

    #endregion EVENT HANDLERS

    #region BUTTON METHODS
    public void OnClickRequestBanner()
    {
        RequestBannerAd();
    }

    public void OnClickDestroyBanner()
    {
        DestroyBannerAd();
    }

    public void OnClickRequestRewardAd()
    {
        RequestAndLoadRewardedAd();
    }

    public void OnClickShowRewardAd()
    {
        ShowRewardedAd();
    }

    public void OnClickRequestInterstitialAd()
    {
        RequestAndLoadInterstitialAd();
    }

    public void OnClickShowInterstitialAd()
    {
        ShowInterstitialAd();
    }

    public void OnClickDestroyInterstitialAd()
    {
        DestroyInterstitialAd();
    }
    #endregion BUTTON METHODS
}
