using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public GameMode gameMode = GameMode.Normal;

    public Vector2Int mapSize;

    public List<BrickData> brickDatas = new List<BrickData>();

    public List<ColorType> hammerColor = new List<ColorType>();

    public Vector3 mapScale;

    public Vector3 mapPosition;

    public float hammerPosition;
}
