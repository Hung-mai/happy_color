using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;

public class Item : MonoBehaviour
{
    [Header("--------type------------")]
    [Searchable]
    public ItemType type;
    [Header("--------transform--------")]
    public Vector3 originalPos;
    public Quaternion OriginalRot;
    public Vector3 OriginalScale;
    [Header("--------Component---------")]
    public bool isChooseItem = false;
    public bool isMerge = false;
    public bool isInBox = true;
    [Header("---------ADD outliner tool--------")]
    public Outlinable outlinable;
    public bool AddOutline_mesh;
    [Header("--------scaleBegin(Code gen)-------")]
    public Vector3 beginLocalScale;
    private void Awake()
    {
        beginLocalScale = transform.localScale;
    }

    [ContextMenu("GenerateAttributeOriginalTransform")]
    public void GenerateAttributeTransform()
    {
        originalPos = transform.localPosition;
        OriginalRot = transform.localRotation;
        OriginalScale = transform.localScale;
    }
    private void OnValidate()
    {
        if (AddOutline_mesh)
        {
            outlinable = GetComponent<Outlinable>();
            if (outlinable == null)
            {
                Debug.LogError("chua co outline trong object nay`!");
                return;
            }
            outlinable.AddAllChildRenderersToRenderingList();
            
            AddOutline_mesh = false;
        }
    }
    public void OnOutline(float timeDelay = 0)
    {
        StartCoroutine(I_Delay(timeDelay));
    }
    IEnumerator I_Delay(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        outlinable.enabled = true;
    }
    public void OffOutline()
    {
        if(outlinable != null) outlinable.enabled = false;
    }
}
