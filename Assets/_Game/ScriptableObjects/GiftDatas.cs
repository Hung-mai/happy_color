using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftData", menuName = "ScriptableObjects/GiftData", order = 1)]
public class GiftDatas : ScriptableObject
{
    public List<GiftInfo> BrickTypeInfo;
    public List<GiftInfo> HammerTypeInfo;
}
