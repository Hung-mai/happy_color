using UnityEngine;
using System.Collections;

public enum RotateAxis
{
    X,
    Y,
    Z
}

public class RotateItem : GameUnit
{
    public RotateAxis axis; 
    public float speed;
    private Transform trans;
    private Vector3 rotation;

    private WaitForSeconds _waitForSeconds;
    // Use this for initialization

    private bool _isPlay;

    void OnEnable()
    {
        _isPlay = true;
        StartCoroutine(UpdateRotate());
    }

    void Start()
    {
        _isPlay = true;
        _waitForSeconds = new WaitForSeconds(1 / 60f);
        trans = transform;
        rotation = trans.localRotation.eulerAngles;
    }

    // Update is called once per frame
    IEnumerator UpdateRotate()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return _waitForSeconds;

            if (_isPlay)
            {
                switch (axis)
                {
                    case RotateAxis.X:
                        rotation.x -= speed;
                        break;
                    case RotateAxis.Y:
                        rotation.y -= speed;
                        break;
                    case RotateAxis.Z:
                        rotation.z -= speed;
                        break;
                }

                trans.localRotation = Quaternion.Euler(rotation);
            }
            
        }
    }

    public void Stop()
    {
        _isPlay = false;

    }

    public void Play()
    {
        _isPlay = true;
    }

    public void Play(Quaternion angle)
    {
        _isPlay = true;

        rotation = angle.eulerAngles;
        trans.localRotation = angle;
    }
}