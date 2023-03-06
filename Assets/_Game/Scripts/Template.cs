using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class Template : SerializedMonoBehaviour
{
    public float heightItem;
    public int y ;
    public int countId;
    [TableMatrix()]
    public int[,] arrayID;

    private int x = 4;
    private int tempY;

    [SerializeField]private bool CreateMatrix;
    private void OnValidate()
    {
        if (!CreateMatrix) return;
        arrayID = new int[x, y];
        tempY = y;
        CreateMatrix = false;
    }

    [Header("--------Columns---------")]
    public Transform[] columns;

    [Header("--------Items---------")]
    public List<Item> items;

    private Vector3 GetPosElement(int x, int _y)
    {
        return new Vector3((columns[x].position.x),transform.position.y + (heightItem * (y-1) - heightItem * _y) ,columns[x].position.z);
    }
    [ContextMenu("Debug Matrix")]
    public void DebugMatrix()
    {
        for (int x = 0; x < arrayID.GetLength(0); x++)
        {
            for (int y = 0; y < arrayID.GetLength(1); y++ )
            {
                Debug.Log("ArrayId["+x+","+y+"] : "+arrayID[x,y].ToString());
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
        foreach (Transform t in columns)
        {
            Gizmos.DrawLine(t.position, t.position + Vector3.up * heightItem * (y));
        }
        for (int i = 0; i < y; i++)
        {
            Gizmos.DrawLine(columns[0].position + Vector3.up * heightItem * i,columns[3].position + Vector3.up * heightItem * i);
        }
        for (int x = 0; x < arrayID.GetLength(0); x++)
        {
            for (int y = 0; y < arrayID.GetLength(1); y++)
            {
                //Debug.Log("ArrayId[" + x + "," + y + "] : " + arrayID[x, y].ToString());
                Handles.Label(GetPosElement(x, y), x + "," + y + " | value : " + arrayID[x,y]);
            }
        }
    }
#endif
    
    public void FillItem(List<Item> itemsInTemplate)
    {
        for (int x = 0; x < arrayID.GetLength(0); x++)
        {
            for (int y = 0; y < arrayID.GetLength(1); y++)
            {
                for (int i = 1; i <= countId; i++)
                {
                    Debug.Log(arrayID[x,y]);
                    if(arrayID[x,y] == i)
                    {
                        Item item = Instantiate(itemsInTemplate[i - 1], transform);
                        item.transform.position = GetPosElement(x,y);
                        
                    }
                }
            }
        }
    }
}
