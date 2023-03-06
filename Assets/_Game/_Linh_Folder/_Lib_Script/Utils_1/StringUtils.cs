using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class StringUtils
{
    public static string Currency = "#,##0";

    public static string ConvertSecondTo_D_H_M_S(float second)
    {
        TimeSpan time = TimeSpan.FromSeconds(second);
        return string.Format(@"{0:00}:{1:00}:{2:00}:{3:00}", time.Days, time.Hours, time.Minutes, time.Seconds);
    }

    public static string ConvertSecondTo_H_M_S(int second)
    {
        TimeSpan time = TimeSpan.FromSeconds(second);
        return string.Format(@"{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
    }

    public static string ConvertSecondTo_M_S(int second)
    {
        TimeSpan time = TimeSpan.FromSeconds(second);
        return string.Format(@"{0:00}:{1:00}", time.Minutes, time.Seconds);
    }

    public static string ConvertSecondTo_M_S_MS(float second)
    {
        TimeSpan time = TimeSpan.FromSeconds(second);
        return string.Format(@"{0:00}:{1:00}:{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
    }

    public static string ConvertSecondTo_M(float second)
    {
        var time = TimeSpan.FromSeconds(second);
        return $@"{0:00}:{time.Minutes:00}";
    }

    public static string ConvertSecondTo_S(float second)
    {
        var time = TimeSpan.FromSeconds(second);
        return $@"{time.Seconds:00}";
    }

    public static string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 15);
    }
}
