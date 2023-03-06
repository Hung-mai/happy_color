using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ClockWatch : MonoBehaviour
{
    public const int AMOUNT = 1000000;
    // Start is called before the first frame update
    void Start()
    {
        AddList();
        AddDict();

        Invoke(nameof(TestList), 5);
        Invoke(nameof(TestDict), 20);
    }

    private List<int> list = new List<int>();
    private Dictionary<Vector2Int, int> dictionary = new Dictionary<Vector2Int, int>();

    public void AddList()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < AMOUNT; i++)
        {
            list.Add(i);
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime add list " + elapsedTime);
    }   
    
    public void AddDict()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < AMOUNT; i++)
        {
            dictionary.Add(new Vector2Int(i,i), i);
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime add dict : " + elapsedTime);
    }

    public void TestList()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < list.Count; i++)
        {
            int t = list[i];
        }

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime add list " + elapsedTime);
    }

    public void TestDict()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        foreach (var item in dictionary)
        {
            int t = item.Value;
        } 

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime add dict : " + elapsedTime);
    }


}
