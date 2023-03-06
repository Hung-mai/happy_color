using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BonusLevelData_", menuName = "ScriptableObjects/BonusLevelData", order = 1)]
public class BonusLevelData : ScriptableObject
{
    public int levelID;
    public bool IsUnlock;
    public int numberOfStar;
    public Sprite levelSampleImage;
}