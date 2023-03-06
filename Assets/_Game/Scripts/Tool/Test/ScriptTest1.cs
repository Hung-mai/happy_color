using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScriptTest1 : MonoBehaviour
{
    public Transform[] cubes;

    public Transform target;
    public List<Vector3> v3s;
    [ContextMenu("DoPathBezier")]
    public void DoPathBezier()
    {
        v3s.Clear();
        float disTarget = Vector3.Distance(cubes[0].position,target.position);
        Vector3 dir = (target.position - cubes[0].position).normalized;
        v3s.Add(cubes[0].position + (dir * disTarget / 2) + Vector3.up * 0.4f);//WP0
        v3s.Add(cubes[0].position + Vector3.up * 0.2f);//1
        v3s.Add(cubes[0].position + (dir * disTarget /4) + Vector3.up * 0.4f);
        v3s.Add(target.position);//Wp1
        
        v3s.Add(cubes[0].position + (dir * disTarget / 4 * 3) + Vector3.up * 0.4f);
        v3s.Add(target.position + Vector3.up * 0.2f);//1/

        cubes[0].DOPath(v3s.ToArray(),3f,PathType.CubicBezier).SetEase(Ease.Linear);

    }
    private void OnDrawGizmosSelected()
    {
        if (v3s.Count == 0) return;
        foreach (Vector3 v in v3s)
        {
            Gizmos.DrawWireSphere(v,0.1f);
        }
    }
}
