using UnityEngine;
using System.Collections;

public class Timer
{
    private static MonoBehaviour behaviour;
    public delegate void Task();

    public static void Schedule(MonoBehaviour _behaviour, float delay, Task task) //Sau "delay" giây thì thực hiện "task" này ở đối tượng "_behaviour" này
    {
        behaviour = _behaviour;
        behaviour.StartCoroutine(DoTask(task, delay));
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return Cache.GetWFS(delay);
        //if(SceneManager.ins.async == null)
        task();
    }
}
