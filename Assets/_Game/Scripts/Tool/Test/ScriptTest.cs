using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScriptTest : MonoBehaviour
{
    public List<int> ints;
    public int indexInsert;
    public int indexRemove;
    public int rangeRemove;
    [ContextMenu("InSert")]
    public void Insert()
    {
        ints.Insert(indexInsert, 111);
    }
    [ContextMenu("Delete")]
    public void Delete()
    {
        ints.RemoveRange(indexRemove, rangeRemove);
    }

    [ContextMenu("PrintChildCount")]
    public void PrintchildCount()
    {
        Debug.Log(transform.childCount);
    }
    [Header("-----------Test Rotate------------")]
    public Transform cube;
    [ContextMenu("rotate")]
    public void rotate()
    {
        cube.transform.DOLocalRotate(new Vector3(cube.transform.localEulerAngles.x + 15f, cube.transform.localEulerAngles.y, cube.transform.localEulerAngles.z), 0.5f).SetEase(Ease.OutQuint);
    }
    [ContextMenu("TestRandom")]
    public void TestRandom()
    {
        Debug.Log(Random.Range(0, 2));
    }
    [Header("------test trung------")]
    public Item sphere1;
    public Item sphere2;
    [ContextMenu("Trung")]
    public void CheckTrung()
    {
        if (sphere1 == sphere2)
        {
            Debug.Log("ok");
        }
    }
    [ContextMenu("TestBreak")]
    public void TestBreak()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int y = 0; y < 10; y++)
            {
                Debug.Log(i);
                if (y== 5)
                {
                    break;
                }
            }
            if (i == 5)
            {
                break;
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Debug.LogError(Camera.main.ViewportToWorldPoint(mousePos));
            Debug.LogError(Camera.main.ScreenToWorldPoint(mousePos));
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
    public List<int> intlst = new List<int>();
    [ContextMenu("ShuffleAList")]
    
    public void huffle()
    {
        intlst.Shuffle();
    }
}
