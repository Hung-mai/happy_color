using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : Singleton<CameraPosition>
{
    public Transform margin;
    public Camera _camera;
    private void Start()
    {
        ChangeCameraPosition();
    }
    public void ChangeCameraPosition()
    {
        Transform tf = transform;
        Vector3 viewPoint = _camera.WorldToViewportPoint(margin.position);
        while (viewPoint.x < 0)
        {
            Debug.Log("CameraMove<0:"+ viewPoint);
            tf.position = tf.position + tf.forward*0.1f;
            viewPoint = _camera.WorldToViewportPoint(margin.position);
        }
        while (viewPoint.x > 1)
        {
            Debug.Log("CameraMove:>1" + viewPoint);
            tf.position = tf.position - tf.forward*0.1f;
            viewPoint = _camera.WorldToViewportPoint(margin.position);
        }
        //tf.position = tf.position - tf.forward * 2f;
    }
}
