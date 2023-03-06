using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

public class TimerUtils
{
    /// <summary>
    /// Bắt buộc phải nhận trong ngày, dùng để hiện popup daily gift
    /// </summary>
    /// <param name="key"></param>
    /// <param name="nextDayCallback"></param>
    public static void DailyGift(string key, Action<string> nextDayCallback)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            nextDayCallback(key);
            PlayerPrefs.SetString(key, DateTime.Now.ToLongDateString());
        }

        var dateTimeInDay = DateTime.Parse(PlayerPrefs.GetString(key));

        if (DateTime.Now.Date.CompareTo(dateTimeInDay.Date) > 0)
        {
            nextDayCallback(key);
            PlayerPrefs.SetString(key, DateTime.Now.ToLongDateString());
        }
    }

    /// <summary>
    /// Qùa trong 1 ngày nếu chưa nhận thì có thể nhận vào lúc khác trong ngày
    /// </summary>
    /// <param name="key"></param>
    /// <param name="nextDayCallback"></param>
    public static void GiftInDay(string key, Action<string> nextDayCallback)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            nextDayCallback(key);
            PlayerPrefs.SetString(key, DateTime.Now.ToLongDateString());
        }

        if (PlayerPrefs.GetInt(key + "_a") == 0)
        {
            nextDayCallback(key);
        }

        var dateTimeInDay = DateTime.Parse(PlayerPrefs.GetString(key));

        if (DateTime.Now.Date.CompareTo(dateTimeInDay.Date) > 0)
        {
            nextDayCallback(key);
            PlayerPrefs.SetInt(key, 0);
            PlayerPrefs.SetString(key, DateTime.Now.ToLongDateString());
        }
    }

    public static void SetGiftInDayReceived(string key)
    {
        PlayerPrefs.SetInt(key + "_a", 1);
    }


    /// <summary>
    /// Thời gian đếm ngược để nhận quà (5 phút, 10 phút...)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    /// <param name="timeCallback"></param>
    /// <param name="endTimeCallback"></param>
    public static void CountDownTime(string key, int time, Action<string, int> timeCallback, Action<string> endTimeCallback)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            PlayerPrefs.SetString(key, DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }

        var dateTimeInDay = DateTime.Parse(PlayerPrefs.GetString(key));

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            var diff1 = DateTime.Now.Subtract(dateTimeInDay);
            var timeRemain = time - (int)diff1.TotalSeconds;

            timeCallback(key, timeRemain);
        }

        if (DateTime.Now.AddSeconds(-time).CompareTo(dateTimeInDay) > 0)
        {
            endTimeCallback(key);
        }
    }

    /// <summary>
    /// Khởi tạo thời gian chạy ngược
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    public static void InitCountdown(string key)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            PlayerPrefs.SetString(key, DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// lặp lại thời gian đếm ngược để nhận quà
    /// </summary>
    /// <param name="key"></param>
    public static void ResetCountDown(string key)
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            PlayerPrefs.SetString(key, DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }
    }

    public static void DeleteCountDown(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    //------------------------------------ TIME OUT GAME ------------------------------------------

    /// <summary>
    /// Set thời gian bắt đầu vào game
    /// </summary>
    /// <param name="key"></param>
    public static void SetTimeGame(string key)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            PlayerPrefs.SetString(key, DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Lấy thời gian từ lúc set time -> hiện tại
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TimeSpan GetTimeGame(string key)
    {
        var dateTimeInDay = DateTime.Parse(PlayerPrefs.GetString(key));

        var diff = DateTime.Now.Subtract(dateTimeInDay);

        return diff;
    }

    /// <summary>
    /// Reset lại thời gian
    /// </summary>
    /// <param name="key"></param>
    public static void ResetTimeGame(string key)
    {
        PlayerPrefs.SetString(key, DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture));
    }
}
