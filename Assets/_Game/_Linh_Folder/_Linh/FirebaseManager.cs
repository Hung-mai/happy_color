using Firebase.Analytics;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Collections;

public enum EventName
{
    checkpoint_start_,
    checkpoint_end_,

    checkpoint_bonus_start_,
    checkpoint_bonus_end_,

    ads_reward_offer,
    ads_reward_click,
    ads_reward_show,
    ads_reward_fail, 
    ads_reward_complete,

    ad_inter_fail,
    ad_inter_load,
    ad_inter_show,
    ad_inter_click,

    level_start,
    level_complete,
    level_fail,

    level_start_bonus,
    level_complete_bonus,

    level_bonus_offer,
    level_bonus_click,
    level_bonus_complete,

    skip_level_click,
    skip_level_show,
    skip_level_complete,

    hint_bonus_level_offer,
    hint_bonus_level_show,
    hint_bonus_level_complete,

    skip_bonus_level_offer,
    skip_bonus_level_show,
    skip_bonus_level_complete,

    ad_impression_abi,
    ads_impression,

    //new Logic
    g_checkpoint_start_,
    g_checkpoint_end_,

    g_gameplay_sort_offer_,
    g_gameplay_sort_skip_,
    g_gameplay_sort_click_,
    g_gameplay_sort_show_,
    g_gameplay_sort_complete_,

    g_level_start,
    g_level_fail,
    g_level_complete,


    g_sortgame_level_start_,
    g_sortgame_level_complete_,

    g_gameplay_booster,

    g_rewarded_ads_offers,
    g_rewarded_ads_clicks,
    g_rewarded_ads_show,
    g_rewarded_ads_completed,

    g_special_offers_show,
    g_special_offers_click,
    g_special_offers_completed,
}

public enum PropertyName
{
    days_played,
    paying_type,
    level_played,
    level_played_1stday,
}

public enum Parameter
{
    level,
    placement,

    ad_platform,
    ad_source,
    ad_unit_name,
    currency,
    country_code,
    ad_format,
    ads,
    value,
    type,
    time,
    booster,
    item_ID,
}

public enum levelType
{
    normal, 
    paint, 
    sort,
}

public enum Booster
{
    skip,
    hint,
}

//reward placement
public enum placement
{
    Bonus_level_offer,
    Bonus_level_hint,
    Bonus_level_skip,
    Level_skip,
    Endgame_Level_Skip,
    Bonus_level_victory,
    Level_Complete,
    Level_Failed,
    Popup_Bonus_Level_Sellect,
    Btn_replay_ingame,
    Sort_level_complete,
    Receive_item_EndGame,
    Receive_Special_item,
    Receive_Explode_item,
}

public enum value
{
    yes,
    no,
}

public class FirebaseManager : MonoBehaviour 
{
    private static FirebaseManager m_Instance = null;
    private bool IsLoaded = false;
    public static FirebaseManager Instance {
        get {
            return m_Instance;
        }
    }
    private FirebaseAnalytics m_FirebaseAnalyticsManager;
    private FirebaseRemoteConfigManager m_FirebaseRemoteConfig;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    private void Awake() {
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    public void Init() {
        if (m_FirebaseAnalyticsManager == null) {
            m_FirebaseAnalyticsManager = new FirebaseAnalytics();
        }
        if (m_FirebaseRemoteConfig == null) {
            m_FirebaseRemoteConfig = new FirebaseRemoteConfigManager();
        }
        Debug.Log("Start Config");
        //if (Application.isEditor) {
        //    InitializeFirebase();
        //    return;
        //};
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                InitializeFirebase();

            }
            else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase() {
        IsLoaded = true;
        m_FirebaseRemoteConfig.SetupDefaultConfigs();
        m_FirebaseRemoteConfig.FetchData(
        () => 
        {
            Debug.Log("Fetch Remote Success");
            m_FirebaseRemoteConfig.InitData();

            if (DataManager.Instance?.gameData.isFirstOpen == true && FirebaseRemoteConfigManager.m_AOA_first_open)  // nếu là ng chơi mới và mở lần đầu  
            {
                AppOpenAdsManager.Instance.WaitShowAOAInSecond(3f);
            }
            else if (DataManager.Instance?.gameData.isFirstOpen == false && FirebaseRemoteConfigManager.m_Enable_AOA) // nếu k là ng chơi mới và mở Aoa
            {
                AppOpenAdsManager.Instance.WaitShowAOAInSecond(3f);
            }
        });
    }

    public bool IsFirebaseReady() {
        return IsLoaded;
    }
    public void LogAnalyticsEvent(string eventName) {
        if (IsFirebaseReady()) {
            m_FirebaseAnalyticsManager.LogEvent(eventName);
        }
    }
    public void LogAnalyticsEvent(string eventName, string eventParamete, double eventValue) {
        if (IsFirebaseReady()) {
            m_FirebaseAnalyticsManager.LogEvent(eventName, eventParamete, eventValue);
        }
    }    

    public void LogAnalyticsEvent(string eventName, string eventParamete, int eventValue) {
        if (IsFirebaseReady()) {
            m_FirebaseAnalyticsManager.LogEvent(eventName, eventParamete, eventValue);
        }
    }    
    
    public void LogAnalyticsEvent(string eventName, string eventParamete, string eventValue) {
        if (IsFirebaseReady()) {
            m_FirebaseAnalyticsManager.LogEvent(eventName, eventParamete, eventValue);
        }
    }

    public void LogAnalyticsEvent(object eventName, object eventParamete, object eventValue) {
        try
        {
            if (IsFirebaseReady())
            {
                m_FirebaseAnalyticsManager.LogEvent(eventName.ToString(), eventParamete.ToString(), eventValue.ToString());
                Debug.LogError(eventName + " | " + eventParamete + " | " + eventValue);
            }
        }catch (System.Exception)
        {

        }
        
    }

    public void LogAnalyticsEvent(string eventName, Firebase.Analytics.Parameter[] paramss) {
        try
        {
            if (IsFirebaseReady())
            {
                m_FirebaseAnalyticsManager.LogEvent(eventName, paramss);
            }
        }
        catch(System.Exception)
        {

        }
    }

    public void LogCheckPoint(string eventname,string level, levelType type)
    {
        try
        {
            if (IsFirebaseReady())
            {
                Firebase.Analytics.Parameter[] paramss = new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter(Parameter.level.ToString(), level),
                    new Firebase.Analytics.Parameter(Parameter.type.ToString(), type.ToString()),
                };
                LogAnalyticsEvent(eventname, paramss);
            }
            Debug.LogError(eventname + " | " + Parameter.level.ToString() + level + " | " + Parameter.type.ToString() + type);
        }
        catch (System.Exception)
        {
        }
    }

    public void LogLevelComplete(string eventname,string level, float time)
    {
        try
        {
            if (IsFirebaseReady())
            {
                Firebase.Analytics.Parameter[] paramss = new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter(Parameter.level.ToString(), level),
                    new Firebase.Analytics.Parameter(Parameter.time.ToString(), time.ToString()),
                };
                LogAnalyticsEvent(eventname, paramss);
            }
            Debug.LogError(eventname + " | " + Parameter.level.ToString() + level + " | " + Parameter.time.ToString() + time);

        }
        catch (System.Exception)
        {
        }
    }

    public void LogGameBooster(Booster booster, string level)
    {
        try
        {
            if (IsFirebaseReady())
            {
                Firebase.Analytics.Parameter[] paramss = new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter(Parameter.booster.ToString(), booster.ToString()),
                    new Firebase.Analytics.Parameter(Parameter.type.ToString(), level.ToString()),
                };
                LogAnalyticsEvent(EventName.g_gameplay_booster.ToString(), paramss);
            }
            Debug.LogError(EventName.g_gameplay_booster.ToString() + booster + " | " + Parameter.booster.ToString() + " | " + Parameter.level.ToString() + level );
        }
        catch (System.Exception)
        {
        }
    }

    #region ADS RevenuePain
    /// <summary>
    /// Send theo event của Max Manager (Log doanh thu từ mỗi quảng cáo)
    /// </summary>
    /// <param name="data"></param>
    public void Log_ADS_RevenuePain(ImpressionData data)
    {
        try
        {
            if (IsFirebaseReady())
            {
                Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter(Parameter.ad_platform.ToString(), "applovin"),
             new Firebase.Analytics.Parameter(Parameter.ad_source.ToString(), data.NetworkName),
             new Firebase.Analytics.Parameter(Parameter.ad_unit_name.ToString(), data.AdUnitIdentifier),
             new Firebase.Analytics.Parameter(Parameter.currency.ToString(), "USD"),
             new Firebase.Analytics.Parameter(Parameter.value.ToString(), data.Revenue),
             new Firebase.Analytics.Parameter(Parameter.placement.ToString(), data.Placement),
             new Firebase.Analytics.Parameter(Parameter.country_code.ToString(), data.CountryCode),
             new Firebase.Analytics.Parameter(Parameter.ad_format.ToString(), data.AdFormat),
             };
                LogAnalyticsEvent(EventName.ad_impression_abi.ToString(), AdParameters);
                LogAnalyticsEvent(EventName.ads_impression.ToString(), AdParameters);
            }
        }
        catch (System.Exception)
        {

        }
        
    }
    #endregion

    public void OnSetUserProperty()
    {
        //Nếu là bản DevelopmentBuild hoặc UnityEditor thì ko bắn UserProperty lên
        if (!Debug.isDebugBuild && !Application.isEditor)
        {

            //int days = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays - DataManager.Instance.gameData.timeInstall;

            //Retention của user, 0 tương ứng D0, 7 tương ứng D7
            //SetUserProperty(PropertyName.days_played.ToString(), ((int)days).ToString());

            //Số reward đã hoàn thành
            SetUserProperty(PropertyName.paying_type.ToString(), DataManager.Instance.gameData.reward_ads_watched.ToString());

            //Số ngày user đã chơi, khác với Retention.Nếu user cài ở D0 và 7 ngày sau mới chơi thì retention là D7 còn days_played là 2
            SetUserProperty(PropertyName.days_played.ToString(), (DataManager.Instance.gameData.days_played).ToString());

            //Số level mà người chơi đã chơi
            SetUserProperty(PropertyName.level_played.ToString(), DataManager.Instance.gameData.levels_played.ToString());

            //Số level người chơi đã chơi ở ngày đầu tiên
            if(DataManager.Instance.gameData.days_played==2&& DataManager.Instance.gameData.levels_played_first_day == 0)
            {
                DataManager.Instance.gameData.levels_played_first_day = DataManager.Instance.gameData.levels_played;
                DataManager.Instance.SaveGame();
            }
            if (DataManager.Instance.gameData.days_played > 1)
            {
                SetUserProperty(PropertyName.level_played_1stday.ToString(), DataManager.Instance.gameData.levels_played_first_day.ToString());
            }
        }
    }

    public void SetUserProperty(string propertyName, string property) {
        if (IsFirebaseReady()) {
            m_FirebaseAnalyticsManager.SetUserProperty(propertyName, property);
        }
    }
}
