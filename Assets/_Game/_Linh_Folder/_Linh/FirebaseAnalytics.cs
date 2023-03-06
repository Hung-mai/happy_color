using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAnalytics {
    public void LogEvent(string eventName) {
        try {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        } catch (System.Exception) {
        }
    }
    public void LogEvent(string eventName, string parameterName, int value) {
        try {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameterName, value);
        } catch (System.Exception) {
        }
    }
    public void LogEvent(string eventName, string parameterName, double value) {
        try {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameterName, value);
        } catch (System.Exception) {
        }
    }  
    public void LogEvent(string eventName, string parameterName, string value) {
        try {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameterName, value);
        } catch (System.Exception) {
        }
    }
    public void LogEvent(string eventName, Firebase.Analytics.Parameter[] param) {
        try {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, param);
        } catch (System.Exception ex) {
            Debug.Log("EX " + ex.Message);
        }
    }
    public void SetUserProperty(string propertyName, string property) {
        try {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(propertyName, property);
        } catch (System.Exception) {
        }
    }
    // public void LogPurchase(double amount, string currencyCode) {
    //     try {
    //         Firebase.Analytics.Parameter[] bundle = new Firebase.Analytics.Parameter[] {
    //             new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCurrency, currencyCode),
    //             new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterValue, amount),
    //         };
    //         Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEcommercePurchase, bundle);
    //     } catch {
    //     }
    // }

}