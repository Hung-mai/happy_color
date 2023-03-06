using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusMaterialDatas", menuName = "ScriptableObjects/BonusMaterialData", order = 1)]
public class BonusMaterialData : ScriptableObject
{
    public List<Material> listMaterials = new List<Material>();
}