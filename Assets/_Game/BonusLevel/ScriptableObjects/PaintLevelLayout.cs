using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusLevelLayout", menuName = "ScriptableObjects/BonusLevelLayout", order = 1)]
public class PaintLevelLayout : ScriptableObject
{
    public LevelLayout[] paintLevelLayout;
}

[System.Serializable]
public class LevelLayout
{
    public string levelName;
    public int levelID;
    public Vector3 cameraPos;
    public Vector3 cameraEndGamePos;
    public Vector3 brickPos;
    public Vector3 brickScale;
    public Vector3 sampleBrickPos;  //Tọa độ tương đối của sample brick lấy Camera làm gốc tọa độ
    public Vector3 sampleBrickScale;
    public Vector3 sampleBrickBGPos; //Ảnh BackGround của mẫu Brick
    public Vector3 sampleBrickBGScale; //Ảnh BackGround của mẫu Brick
}
