using AppsFlyerSDK;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppLovinController : MonoBehaviour {
    public static AppLovinController instance;
    private string SDK_Key = "ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ";
    private string AndroidBannerID = "1eb1dd4b94b48a92"; //1eb1dd4b94b48a92
    private string AndroidInterID = "abe2f28ed5256229";
    [HideInInspector]public string AndroidRewardID = "fb1cb363b784cf94";

    public const string AF_LEVEL = "af_level";

    public const float INTER_COUNTDOWN_TIME = 20f;
    public const float AOA_COUNTDOWN_TIME = 10f;

    float countdownInterTime = 0;
    float loadAdsCountingTime = 0;
    int NumberOfInterAds = 0;

    UnityAction RewardSuccessCallback;
    UnityAction RewardCloseCallback;
    UnityAction RewardFailCallback;

    private placement interPlacement;
    private placement rewardPlacement;

    Dictionary<string, string> evenParams = new Dictionary<string, string>();

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private bool hasAOA = false;

    // Start is called before the first frame update
    void Start() {
        if (GameManager.Instance.IsNoAds) return;
        StartCoroutine(asyncAds());
    }

    IEnumerator asyncAds()
    {
        // nếu có show thì 
        if (DataManager.Instance?.gameData.isFirstOpen == true && FirebaseRemoteConfigManager.m_AOA_first_open)  // nếu là ng chơi mới và mở lần đầu  
        {
            hasAOA = true;
            AppOpenAdsManager.Instance.LoadAd();
        }
        else if (DataManager.Instance?.gameData.isFirstOpen == false && FirebaseRemoteConfigManager.m_Enable_AOA) // nếu k là ng chơi mới và mở Aoa
        {
            hasAOA = true;
            AppOpenAdsManager.Instance.LoadAd();
        }

        if (hasAOA == true)
        {
            yield return new WaitUntil(() => AppOpenAdsManager.Instance.IsAdAvailable || GameManager.Instance?.timeCounting > 4);
        }

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            ShowBanner();
            Debug.Log("Ads Initialize");
        };
        MaxSdk.SetSdkKey(SDK_Key);
        MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
        MaxSdk.InitializeSdk();
        //MaxSdk.ShowMediationDebugger();
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AndroidRewardID));
        if(CanvasLoading.ins!=null) CanvasLoading.ins.isAdsLoaded = true;
    }

    private void Update()
    {
        loadAdsCountingTime += Time.deltaTime;
        if (countdownInterTime > 0)
        {
            countdownInterTime -= Time.deltaTime;
        }
    }

    #region Banner

    public void InitializeBannerAds() {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(AndroidBannerID, MaxSdkBase.BannerPosition.BottomCenter);
        // MaxSdk.SetBannerBackgroundColor(AndroidBannerID, Color.black);
    }

    public void ShowBanner() {
        if(GameManager.Instance.IsNoAds) return;
        MaxSdk.ShowBanner(AndroidBannerID);
    }
    public void HideBanner() {
        MaxSdk.HideBanner(AndroidBannerID);
    }
    public void InitializeInterstitialAds() {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
        // Load the first interstitial
        LoadInterstitial();
    }

    #endregion

    #region Inter

    private void LoadInterstitial() {
        MaxSdk.LoadInterstitial(AndroidInterID);
        Debug.Log("Start Load Ads Inter");
        loadAdsCountingTime = 0;
    }

    public void ShowInterstitial() {
        Debug.Log("Ads Inter click");
        if (countdownInterTime > 0)
        {
            Debug.Log("Don't show Ads: Remain CappingTime = " + countdownInterTime);
            return;
        }
        if (GameManager.Instance.IsNoAds)
        {
            Debug.Log("GameManager.Instance.IsNoAds = " + GameManager.Instance.IsNoAds);
            return;
        }
#if UNITY_EDITOR
        Debug.Log("UNITY_EDITOR = true");
        return;
#endif
        AppsFlyer.sendEvent(AppsFlyer.af_inters_ad_eligible, evenParams);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ad_inter_click, Parameter.placement, interPlacement);
        Debug.Log("Call Show Interstitial");
        if (MaxSdk.IsInterstitialReady(AndroidInterID))
        {
            Debug.Log("Inter ready, show inter level:"+ DataManager.Instance.gameData.allLevelCounting+"| Capping="+ countdownInterTime);
            countdownInterTime = INTER_COUNTDOWN_TIME;
            AppOpenAdsManager.Instance.IsCanOpenAOA = false;
            MaxSdk.ShowInterstitial(AndroidInterID);
        }
        else
        {
            Debug.Log("Inter not ready, reload inter");
            LoadInterstitial();
        }
    }

    #endregion

    #region Reward


    public void InitializeRewardedAds() {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd() {
        MaxSdk.LoadRewardedAd(AndroidRewardID);
    }

    public void SetInterPlacement(placement placement)
    {
        interPlacement = placement;
    }

    public void SetRewardPlacement(placement placement)
    {
        rewardPlacement = placement;
    }

    public void ShowRewardedAd(UnityAction callback, UnityAction closeCallback = null, UnityAction failCallback = null) {
        
#if UNITY_EDITOR
    callback?.Invoke();
    closeCallback?.Invoke();
    return;
#endif

        if (rewardPlacement == placement.Receive_item_EndGame) FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_offer, Parameter.placement, placement.Receive_item_EndGame);
        if (rewardPlacement == placement.Receive_Special_item) FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_offer, Parameter.placement, placement.Receive_Special_item);
        if (rewardPlacement == placement.Receive_Explode_item) FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_offer, Parameter.placement, placement.Receive_Explode_item);
        if(GameManager.Instance.IsNoAds)
        {
            callback?.Invoke();
            closeCallback?.Invoke();
            return;
        }
        AppsFlyer.sendEvent(AppsFlyer.af_rewarded_ad_eligible, evenParams);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_click, Parameter.placement, rewardPlacement);

        if (rewardPlacement == placement.Bonus_level_offer) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_click_+ DataManager.Instance.gameData.sortLevelOfferTimes.ToString(), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        if (rewardPlacement == placement.Receive_item_EndGame) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_rewarded_ads_clicks, Parameter.item_ID, DataManager.Instance.gameData.currentGiftName);
        if (rewardPlacement == placement.Receive_Special_item) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_special_offers_click, Parameter.item_ID, DataManager.Instance.gameData.currentSpecialItemName);
        
        RewardSuccessCallback = callback;
        RewardCloseCallback = closeCallback;
        RewardFailCallback = failCallback;
        try
        {
            if (MaxSdk.IsRewardedAdReady(AndroidRewardID)) {
                countdownInterTime = INTER_COUNTDOWN_TIME;
                AppOpenAdsManager.Instance.IsCanOpenAOA = false;
                MaxSdk.ShowRewardedAd(AndroidRewardID);
            }
            else
            {
                RewardFailCallback?.Invoke();
                RewardFailCallback = null;
                LoadRewardedAd();
            }
        }
        catch (System.Exception)
        {
            RewardFailCallback?.Invoke();
            RewardFailCallback = null;
        }
    }

    private IEnumerator WaitUntilShowReward(UnityAction firstOpenClickEvent)
    {
        for (int i = 0; i < 15; i++)
        {
            try
            {
                if (MaxSdk.IsRewardedAdReady(AndroidRewardID)) {
                    countdownInterTime = INTER_COUNTDOWN_TIME;
                    AppOpenAdsManager.Instance.IsCanOpenAOA = false;
                    firstOpenClickEvent?.Invoke();
                    MaxSdk.ShowRewardedAd(AndroidRewardID);
                    break;
                }
            }
            catch {}

            yield return Cache.GetWFS(0.2f);
        }
    }

    #endregion

    int retryAttemptIntern;
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        // Reset retry attempt
        Debug.Log("Thoi gian load Ads Inter: " + loadAdsCountingTime);
        NumberOfInterAds++;
        Debug.Log("Number Ads Inter Loaded: " + NumberOfInterAds);
        retryAttemptIntern = 0;
        loadAdsCountingTime = 0;
        AppsFlyer.sendEvent(AppsFlyer.af_inters_api_called, evenParams);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ad_inter_load, Parameter.placement, interPlacement);
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        Debug.Log("Interstitial Load Failed Event: " + errorInfo);
        retryAttemptIntern++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptIntern));
        Invoke(nameof(LoadInterstitial), (float)retryDelay);

    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        Debug.Log("Interstitial Displayed Event: " + adInfo);
        AppsFlyer.sendEvent(AppsFlyer.af_inters_displayed, evenParams);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ad_inter_show, Parameter.placement, interPlacement);
        AppOpenAdsManager.Instance.IsCanOpenAOA = false;
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        Debug.Log("Interstitial Ad Failed To Display Event: " + errorInfo);
        LoadInterstitial();
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ad_inter_fail, Parameter.placement, interPlacement);
        AppOpenAdsManager.Instance.IsCanOpenAOA = false;
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        countdownInterTime = INTER_COUNTDOWN_TIME;
        AppOpenAdsManager.Instance.ResetTimeAOA();
    }

    private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "interstitial";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = interPlacement.ToString();//placement;
        data.Revenue = revenue;
        FirebaseManager.Instance.Log_ADS_RevenuePain(data);
    }


    int retryAttemptReward;
    bool recieveReward = false;
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        // Reset retry attempt
        Debug.Log("Thoi gian load Ads Reward: " + loadAdsCountingTime);
        retryAttemptIntern = 0;
        AppsFlyer.sendEvent(AppsFlyer.af_rewarded_api_called, evenParams);
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        retryAttemptIntern++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptIntern));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }
    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        recieveReward = false;
        AppsFlyer.sendEvent(AppsFlyer.af_rewarded_ad_displayed, evenParams);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_show, Parameter.placement, rewardPlacement);
        if (rewardPlacement == placement.Bonus_level_offer) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_show_ + DataManager.Instance.gameData.sortLevelOfferTimes.ToString(), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        if (rewardPlacement == placement.Level_skip) 
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_level_show, Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
            FirebaseManager.Instance.LogGameBooster(Booster.skip,(DataManager.Instance.gameData.allLevelCounting+1).ToString());
        }
        if (rewardPlacement == placement.Bonus_level_hint) 
        {
            FirebaseManager.Instance.LogGameBooster(Booster.hint, (DataManager.Instance.gameData.allLevelCounting + 1).ToString());
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.hint_bonus_level_show, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        }
        if (rewardPlacement == placement.Bonus_level_skip) 
        {
            FirebaseManager.Instance.LogGameBooster(Booster.skip, (DataManager.Instance.gameData.allLevelCounting + 1).ToString());
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_bonus_level_show, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        }
        if (rewardPlacement == placement.Receive_item_EndGame) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_rewarded_ads_show, Parameter.item_ID, DataManager.Instance.gameData.currentGiftName);
        AppOpenAdsManager.Instance.IsCanOpenAOA = false;
        Debug.Log("reward display");
    }
    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
        RewardFailCallback?.Invoke();
        RewardFailCallback = null;

        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_fail, Parameter.placement, rewardPlacement);
        AppOpenAdsManager.Instance.IsCanOpenAOA = false;
    }
    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
    }
    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        RewardCloseCallback?.Invoke();
        RewardCloseCallback = null;
        countdownInterTime = INTER_COUNTDOWN_TIME;
        AppOpenAdsManager.Instance.ResetTimeAOA();

        if(recieveReward == false)
        {
            RewardFailCallback?.Invoke();
            RewardFailCallback = null;
        }
        else // true
        {
            recieveReward = false;
        }
    }
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) {
        // The rewarded ad displayed and the user should receive the reward.
        recieveReward = true;
        RewardSuccessCallback?.Invoke();
        RewardSuccessCallback = null;

        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_complete, Parameter.placement, rewardPlacement);
        if (rewardPlacement == placement.Bonus_level_offer) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_complete_ + DataManager.Instance.gameData.sortLevelOfferTimes.ToString(), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        if (rewardPlacement == placement.Bonus_level_offer) FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_bonus_complete, Parameter.level, DataManager.Instance.gameData.currentPaintLevel.ToString());
        if (rewardPlacement == placement.Level_skip) FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_level_complete, Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
        if (rewardPlacement == placement.Bonus_level_hint) FirebaseManager.Instance.LogAnalyticsEvent(EventName.hint_bonus_level_complete, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        if (rewardPlacement == placement.Bonus_level_skip) FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_bonus_level_complete, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        if (rewardPlacement == placement.Receive_Special_item) FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_special_offers_completed, Parameter.item_ID, DataManager.Instance.gameData.currentSpecialItemName);
        DataManager.Instance.gameData.reward_ads_watched++;
        DataManager.Instance.SaveGame();
        AppsFlyer.sendEvent(AppsFlyer.af_rewarded_ad_completed, evenParams);
    }
    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Ad revenue paid. Use this callback to track user revenue.
        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "video_reward";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = rewardPlacement.ToString();//placement;
        data.Revenue = revenue;
        FirebaseManager.Instance.Log_ADS_RevenuePain(data);
        Debug.Log("Rewarded ad revenue paid");
    }

    public void AppsflyerSentEventLevelAchieve()    
    {
        evenParams.Add(AF_LEVEL, DataManager.Instance.gameData.levels_played.ToString()); ;
        AppsFlyer.sendEvent(AppsFlyer.af_level_achieved, evenParams);
        evenParams.Clear();
    }

}

[System.Serializable]
public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;
}

