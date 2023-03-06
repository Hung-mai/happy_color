using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataToypickerLevel", menuName = "Data/DataToyPickerLevel", order = 1)]
public class DataLevelBonus : ScriptableObject
{
    public int level;
    public LevelBonus prefabLevel;
}
