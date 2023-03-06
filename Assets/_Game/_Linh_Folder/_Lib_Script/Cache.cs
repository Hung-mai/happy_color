using UnityEngine;
using System.Collections.Generic;


public class Cache
{

    private static Dictionary<float, WaitForSeconds> m_WFS = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWFS(float key)
    {
        if(!m_WFS.ContainsKey(key))
        {
            m_WFS[key] = new WaitForSeconds(key);
        }

        return m_WFS[key];
    }

    //------------------------------------------------------------------------------------------------------------


    private static Dictionary<Collider, Hammer> m_Hammer = new Dictionary<Collider, Hammer>();

    public static Hammer GetHammer(Collider key)
    {
        if (!m_Hammer.ContainsKey(key))
        {
            Hammer hammer = key.GetComponent<Hammer>();

            if (hammer != null)
            {
                m_Hammer.Add(key, hammer);
            }
            else
            {
                return null;
            }
        }

        return m_Hammer[key];
    }


}
