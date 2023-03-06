using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenAdsManager : MonoBehaviour
{
    public const float AOA_RELOAD_TIME = 15f;

#if UNITY_ANDROID
    private string AD_UNIT_ID = "ca-app-pub-9819920607806935/8847444144";    // key đúng
    // private string AD_UNIT_ID = "ca-app-pub-3940256099942544/3419835294";       // key test
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5662855259";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif

    private static AppOpenAdsManager instance;

    private AppOpenAd ad;

    private bool isShowingAd = false;

    private float waitTime = 0;

    DateTime lastViewAOATime = DateTime.MinValue;

    private bool isCanOpen = false;

    public bool IsCanOpenAOA
    {
        get
        {
            return isCanOpen;
        }
        set
        {
            isCanOpen = value;
        }
    }

    public static AppOpenAdsManager Instance {
        get 
        {
            if (instance == null) 
            {
                instance = FindObjectOfType<AppOpenAdsManager>();
            }

            if (instance == null)
            {
                instance = new AppOpenAdsManager();
            }

            return instance;
        }
    }
    public bool IsAdAvailable {
        get {
            return ad != null;
        }
    }
    private void Awake() {
        MobileAds.Initialize(initStatus => { });
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;

            if (IsAdAvailable && !isShowingAd)
            {
                waitTime = 0;
                if(!GameManager.Instance.IsNoAds) ShowAdIfAvailable();
            }
        }

    }

    public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded
        if (!paused && !IsCanOpenAOA)
        {
            IsCanOpenAOA = true;
            return;
        }

        if (paused)
        {
            ResetTimeAOA();
            return;
        }

        //if tat aoa
        //tat switch aoa
        //chua du tgian
        //return
        if (!FirebaseRemoteConfigManager.m_Enable_AOA
            || !FirebaseRemoteConfigManager.m_AOA_SwitchApps
            || DateTime.Now.Subtract(lastViewAOATime).TotalSeconds < FirebaseRemoteConfigManager.m_AOA_SwitchApps_time)
            return;
        if (!paused)
        {
            WaitShowAOAInSecond(2f);
        }
    }

    public void LoadAd() {
        AdRequest request = new AdRequest.Builder().Build();
        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) => {
            if (error != null) {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }
            // App open ad is loaded.
            ad = appOpenAd;
        }));
    }

    public void WaitShowAOAInSecond(float waitTime)
    {
        this.waitTime = waitTime;
    }

    public void ShowAdIfAvailable() {
        Debug.Log("ShowAdIfAvailable");
        Debug.Log("AppOpenAd Avaiable: " + IsAdAvailable);
        if (!IsAdAvailable || isShowingAd) {
            return;
        }
        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        ad.OnPaidEvent += HandlePaidEvent;
        ad.Show();
        //Debug.LogError("Show AOA");
        Debug.Log("ShowAdIfAvailable Done!");
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args) {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        Invoke(nameof(OnResetAOA), 2.5f);
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args) {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args) {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args) {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args) {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }

    private void OnResetAOA()
    {
        isShowingAd = false;
        ResetTimeAOA();
        LoadAd();
    }

    public void ResetTimeAOA()
    {
        lastViewAOATime = DateTime.Now;
    }
}
