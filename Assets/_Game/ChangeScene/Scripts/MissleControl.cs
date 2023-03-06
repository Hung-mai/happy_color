using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleControl : MonoBehaviour
{
    public Transform targetpoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime*20f);
        Vector3 missleDirection = targetpoint.position - transform.position;
        missleDirection = missleDirection.normalized;
        Quaternion missleRot = Quaternion.LookRotation(missleDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, missleRot, 5 * Time.deltaTime);
    }
}
