using System;
using System.Collections;
using UnityEngine;

public class RoutineUtils : MonoBehaviour 
{
    public static IEnumerator Run(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();

        callback = null;
    }

    public static IEnumerator Run(float time, int index, Action<int> callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke(index);

        callback = null;
    }
}
