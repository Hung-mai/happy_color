using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;

public class FirebaseRemoteConfigManager 
{
    public const string Enable_AOA = "Enable_AOA";
    public const string AOA_first_open = "AOA_first_open";
    public const string AOA_SwitchApps = "AOA_SwitchApps";
    public const string AOA_SwitchApps_time = "AOA_SwitchApps_time";
    public const string Enable_PopupRate = "Enable_PopupRate";
    public const string Inter_first_game = "Inter_first_game";
    public const string Inter_from_level = "Inter_from_level";  
    public const string Show_inter_mode = "Show_inter_mode";   

    public static bool m_Enable_AOA = true;
    public static bool m_AOA_first_open = false;
    public static bool m_AOA_SwitchApps = true;
    public static double m_AOA_SwitchApps_time = 15f;
    public static bool m_Enable_PopupRate = true;
    public static bool m_Inter_first_game = false;
    public static double m_Inter_from_level = 4f;       //Level bắt đầu show inter
    public static bool m_Show_inter_mode = true;       //Chế độ show inter

    internal static bool m_IsLoaded = false;

    private UnityAction m_FetchSuccessCallback;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public void SetupDefaultConfigs(Dictionary<string, object> defaults) {
        if (defaults == null) return;
        try {
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            ConfigSettings cs = FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
#if UNITY_EDITOR
            //cs.IsDeveloperMode = true;
#endif
        } catch (System.Exception) {
        }
    }
    public ConfigValue GetValues(string key) {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
    }

    public void FetchData(UnityAction fetchSuccessCallback) {
        m_FetchSuccessCallback = fetchSuccessCallback;
        // FetchAsync only fetches new data if the current data is older than the provided
        // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
        // By default the timespan is 12 hours, and for production apps, this is a good
        // number.  For this example though, it's set to a timespan of zero, so that
        // changes in the console will always show up immediately.
        try {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            fetchTask.ContinueWith(FetchComplete);

        } catch (Exception) {
        }
    }
    void FetchComplete(Task fetchTask) {
        if (fetchTask.IsCanceled) {
            Debug.Log("Fetch canceled.");
        } else if (fetchTask.IsFaulted) {
            Debug.Log("Fetch encountered an error.");
        } else if (fetchTask.IsCompleted) {
            Debug.Log("Fetch completed successfully!");
        }
        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus) {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
                Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                    info.FetchTime));
                if (m_FetchSuccessCallback != null) {
                    m_FetchSuccessCallback();
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason) {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }
    public void SetupDefaultConfigs() {
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        defaults.Add(Enable_AOA, true);
        defaults.Add(AOA_first_open, false);
        defaults.Add(AOA_SwitchApps, true);
        defaults.Add(AOA_SwitchApps_time, 15);
        defaults.Add(Enable_PopupRate, true);
        defaults.Add(Inter_first_game, false);

        SetupDefaultConfigs(defaults);
    }

    public void InitData()
    {
        m_Enable_AOA = GetValues(Enable_AOA).BooleanValue;
        m_AOA_first_open = GetValues(AOA_first_open).BooleanValue;
        m_AOA_SwitchApps = GetValues(AOA_SwitchApps).BooleanValue;
        m_AOA_SwitchApps_time = GetValues(AOA_SwitchApps_time).DoubleValue;
        m_Enable_PopupRate = GetValues(Enable_PopupRate).BooleanValue;
        m_Inter_first_game = GetValues(Inter_first_game).BooleanValue;
        m_Inter_from_level = GetValues(Inter_from_level).DoubleValue;
        m_Show_inter_mode = GetValues(Show_inter_mode).BooleanValue;



        // Debug.LogError("?? m_Enable_AOA: " + m_Enable_AOA);
        // Debug.LogError("?? m_AOA_first_open: " + m_AOA_first_open);
        // Debug.LogError("?? m_AOA_SwitchApps: " + m_AOA_SwitchApps);
        // Debug.LogError("?? m_AOA_SwitchApps_time: " + m_AOA_SwitchApps_time);
        // Debug.LogError("?? m_Enable_PopupRate: " + m_Enable_PopupRate);
        // Debug.LogError("?? m_Inter_first_game: " + m_Inter_first_game);

        m_IsLoaded = true;
    }
}
