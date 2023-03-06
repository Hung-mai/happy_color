using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(Camera))]
public class CameraShake : Singleton<CameraShake>
{
    [Header("-------CamStatOriginal--------")]
    public Vector3 pos;
    public Quaternion rot;

    public float duration = 0.15f;
    public float streng = 0.005f;

    
    private Camera cam;
    public void Awake()
    {
        cam = GetComponent<Camera>();
    }
    public void Shake()
    {
        cam.DOShakePosition(duration, streng, fadeOut: true).OnComplete(() => {
            cam.transform.DOMove(pos,0.1f);
            cam.transform.DORotateQuaternion(rot,0.1f);
        });
        cam.DOShakeRotation(duration, streng, fadeOut: true);
    }
}
