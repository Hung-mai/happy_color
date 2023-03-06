using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : Singleton<CameraControl>
{
    public Transform margin;
    public Camera _camera;
    public Transform tf;
    public void ChangeCameraPosition()
    {
        tf = transform;
        Vector3 viewPoint = _camera.WorldToViewportPoint(margin.position);
        while (viewPoint.x < 0)
        {
            tf.position = tf.position - tf.forward;
            viewPoint = _camera.WorldToViewportPoint(margin.position);
        }
        tf.position = tf.position - tf.forward;
    }
}
