using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountryUtils
{
    public static List<Sprite> CountryList;


    /// <summary>
    /// Nạp tất cả country
    /// </summary>
    public static void LoadAllCountries()
    {
        CountryList = Resources.LoadAll<Sprite>("Flags").ToList();
    }

    public static List<Sprite> GetRandomCountries(int take)
    {
        return CountryList.OrderBy(d => System.Guid.NewGuid()).Take(take).ToList();
    }
}
