using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusBrickData
{
    public Vector3Int colorID;  //Chứa mã màu RGB
    public List<Vector2Int> listBonusBrickID = new List<Vector2Int>();  //List chứa ID của tất cả các brick có mã màu trên
}
