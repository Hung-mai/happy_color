using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchTest : MonoBehaviour
{
    public List<int> listINT = new List<int>();

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            listINT.Add(i);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int tempGO;
            for (int i = 0; i < listINT.Count; i++)
            {
                int rnd = UnityEngine.Random.Range(0, listINT.Count);
                tempGO = listINT[rnd];
                listINT[rnd] = listINT[i];
                listINT[i] = tempGO;
            }
        }
    }
}
