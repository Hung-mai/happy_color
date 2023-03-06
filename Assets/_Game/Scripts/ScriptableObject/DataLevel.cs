using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataLevel", menuName = "Data/DataLevel", order = 1)]
public class DataLevel : ScriptableObject
{
    public int level;
    public Level prefabLevel;

    [Header("--------MapConfig_Stat--------")]
    public StatTransform leftContainer = new StatTransform(Vector3.zero,Quaternion.Euler(0,0,0),Vector3.one);
    public StatTransform rightContainer = new StatTransform(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
    public StatTransform backContainer = new StatTransform(new Vector3(0, 19.9799995f, 0.100000001f), Quaternion.Euler(0, 0, 0), new Vector3(2.26250339f, 40.9512482f, 0.06795834f));
    public StatTransform glassFront_Top = new StatTransform(new Vector3(-0.0251093134f, 0.023586018f, -0.0073405453f), Quaternion.Euler(0, 0, 0), Vector3.one);
    public StatTransform glassFront_Bot = new StatTransform(new Vector3(0, 0.119999997f, -1.31429994f), Quaternion.Euler(0, 0, 0), new Vector3(2.20594072f, 1.46725059f, 0.0452096127f));
    
    [Header("trigger reset config default")]public bool isReset = false;
   
    private void OnValidate()
    {
        if (!isReset) return;
        leftContainer = new StatTransform(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
        rightContainer = new StatTransform(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
        backContainer = new StatTransform(new Vector3(0, 19.9799995f, 0.100000001f), Quaternion.Euler(0, 0, 0), new Vector3(2.26250339f, 40.9512482f, 0.06795834f));
        glassFront_Top = new StatTransform(new Vector3(-0.0251093134f, 0.023586018f, -0.0073405453f), Quaternion.Euler(0, 0, 0), Vector3.one);
        glassFront_Bot = new StatTransform(new Vector3(0, 0.119999997f, -1.31429994f), Quaternion.Euler(0, 0, 0), new Vector3(2.20594072f, 1.46725059f, 0.0452096127f));
        
    }
}
[System.Serializable]
public class StatTransform
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public StatTransform(Vector3 p , Quaternion r, Vector3 s)
    {
        pos = p;
        rot = r;
        scale = s;
    }
}
