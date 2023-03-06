using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public Transform tf;
    public Transform child;
    public Transform mainCube;
    public AnimationCurve curve;
    public ColorData colorData;

    public MeshRenderer meshRenderers;

    internal BrickData brickData;

    public Vector2Int ID { get => id; }

    public bool IsClear => brickData == null || brickData.colorType == ColorType.Clear;

    public ColorType ColorType => brickData == null ? ColorType.Clear : brickData.colorType;

    public ColorType nextLevelColor = ColorType.Clear;
    public GameObject shadow;
    public GameObject stage;
    public GameObject mainCubegob;


    internal Vector2Int id;

    private Vector3 point;
    private float TimeAlive = 0.5f;
    private float TimeChangeColor = 0.2f;
    private float time = 0;
    private bool isChangeParent;

    public BoxCollider collider;
    public Rigidbody rigidbody;

    private float timeCounting = 0;

    //ChangeCube
    public Transform[] mainCubeList;
    public MeshRenderer[] meshRenderersList;
    public GameObject[] mainCubegobList;
    public BoxCollider[] colliderList;
    public Rigidbody[] rigidbodyList;

    void Start()
    {
        collider.enabled = false;
        rigidbody.useGravity = false;
    }

    public void SetPhysics(bool IsPhysics)
    {
        collider.enabled = IsPhysics;
        rigidbody.useGravity = IsPhysics;
    }

    public void SetID(Vector2Int id)
    {
        this.id = id;
    }

    public void ChangeColor(ColorType colorType)
    {
        if (colorType == ColorType.Editor)
        {
            colorType = ColorType.Clear;
        }

        stage.SetActive(colorType != ColorType.Clear);
        Material colorMat = colorData.GetColorMat(colorType);
        meshRenderers.material = colorMat;
    }

    public void SetParent(BrickData brickData)
    {
        this.brickData = brickData;
    }

    public bool IsCollect(Vector3 point)
    {
        return Vector3.Distance(point, tf.position) < 0.1f;
    }

    internal void OnInit()
    {
        time = 0;
        this.brickData = null;
        ChangeColor(ColorType.Clear);
    }

    #region Wave

    UnityAction OnTopWaveAction, OnExitAction;

    public void OnEnter(UnityAction OnEnterAction, UnityAction OnTopWaveAction, UnityAction OnExitAction)
    {
        time = 0;
        isChangeParent = false;
        //Control.Instance.AddWave(this);
        OnEnterAction?.Invoke();
        this.OnTopWaveAction = OnTopWaveAction;
        this.OnExitAction = OnExitAction;
    }

    public void OnExecute()
    {
        if (time < TimeAlive)
        {
            time += Time.deltaTime;

            if (!isChangeParent && time >= TimeChangeColor)
            {
                isChangeParent = true;
                OnTopWaveAction?.Invoke();
                //ChangeColor(brickData.colorType);
            }

            if (time >= TimeAlive)
            {
                time = TimeAlive;
                OnExit();
            }

            point.y = curve.Evaluate(time);
            child.localPosition = point;
        }
    }

    public void OnExit()
    {
        OnExitAction?.Invoke();
        //Control.Instance.RemoveWave(this);
    }
    public void AddWave()
    {
        Control.Instance.AddWave(this);
    }

    public void ChangeColor()
    {
        ChangeColor(brickData.colorType);
    }

    public void RemoveWave()
    {
        Control.Instance.RemoveWave(this);
    }

    #endregion

    #region WinWave

    public void AddWinWave()
    {
        Control.Instance.AddWave(this);
    }

    public void RemoveWinWave()
    {
        Control.Instance.RemoveWinWave(this);
    }

    #endregion

    public void ChangeBrickType(BrickType brickType)
    {
        mainCubegob.SetActive(false);

        //Set Next Brick Info
        meshRenderersList[(int)brickType].material = meshRenderers.material;
        //Change BrickType
        mainCube = mainCubeList[(int)brickType];
        meshRenderers = meshRenderersList[(int)brickType];
        mainCubegob = mainCubegobList[(int)brickType];
        collider = colliderList[(int)brickType];
        rigidbody = rigidbodyList[(int)brickType];

        mainCubegob.SetActive(true);
    }
}
