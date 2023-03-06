using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEditor;

public class LoadMainGameData : Singleton<LoadMainGameData>
{
#if UNITY_EDITOR
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    public List<TextAsset> dataText;
    public TextAsset[] fewDataText;
    public Vector2Int mapSize;
    public List<int> listAllColorID;
    private Dictionary<Vector2Int, Vector3Int> listBrickIDColorID = new Dictionary<Vector2Int, Vector3Int>();    //Vector2Int là tọa độ brick, Vector3Int là mã màu rgb
    public Dictionary<Vector2Int, ColorType> listBrickIDColorType = new Dictionary<Vector2Int, ColorType>();    //Vector2Int là tọa độ brick, 
    private List<Vector3Int> colorList = new List<Vector3Int>();
    public LevelData levelData;
    private BrickData tempBrickData = new BrickData();
    private List<BrickData> tempBrickDataList = new List<BrickData>();

    //Hammer Data Reader
    public TextAsset hammerDataText;
    public List<HammerColor> listLevelHammer = new List<HammerColor>(); //List chứa tất cả các màu của hammer trong từng level
    private List<ColorType> tempListHammerColor = new List<ColorType>(); //Màu của tất cả các búa ở 1 level. Dùng để hỗ trợ quá trình đọc file Hammer data
    private string[] tempHammerData;

    //Layout Data Reader
    public TextAsset levelLayoutDatas;
    public List<Vector3> scaleDatas;
    public List<Vector3> positionDatas;
    public List<float> hammerPositionDatas;
    private string[] tempLayoutData;

    public bool createFromFewDatas;

    //Editor
    public Brick SellectingBrick;
    public int rayCastDistance;

    private void OnEnable()
    {
        if(!GameManager.Instance.IsEditor) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
    public void ReadData()
    {
        Reset();
        //Load map data text
        if (createFromFewDatas)
        {
            for (int i = 0; i < fewDataText.Length; i++)
            {
                dataText.Add(fewDataText[i]);
            }
        }
        else
        {
            TextAsset tempData = null;
            int index = 1;
            while (true)
            {
                tempData = Resources.Load<TextAsset>("Level_Data_New_" + index);
                if (tempData == null) break;
                dataText.Add(tempData);
                index++;
            }
        }
        
        //ReadHammerData
        HammerDataReader();

        //Read Layout Data
        ReadLayoutData();

        //Read CSV Data
        Debug.Log("Read Files:"+ dataText.Count);
        for (int i = 0; i < dataText.Count; i++)
        {
            CSVReader(i);
        }
    }

    #region Read Hammer datas CSV file
    private void HammerDataReader()
    {
        //Read map data
        tempHammerData = hammerDataText.text.Split(new string[] { "", " ", ",", "\n", ('"').ToString(), LINE_SPLIT_RE }, StringSplitOptions.RemoveEmptyEntries);
        string[] lineCount = hammerDataText.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);    //Là số lần xuống dòng (Dùng để đếm số hàng của file excell)
        Debug.Log("Xuong dong:"+ lineCount.Length);
        int index = 1;
        Vector3Int colorID = Vector3Int.zero;
        for (int i = 0; i < tempHammerData.Length; i++)
        {
            try
            {
                if (index == 1) colorID.x = int.Parse(tempHammerData[i]);
                else if (index == 2) colorID.y = int.Parse(tempHammerData[i]);
                else if (index == 3)
                {
                    colorID.z = int.Parse(tempHammerData[i]);
                    if (FindColorType(colorID) != ColorType.Clear) tempListHammerColor.Add(FindColorType(colorID));
                    else Debug.Log("Wrong RGB Color ID:"+colorID);
                    colorID = Vector3Int.zero;
                } 

                index++;
                if (index == 4) index = 1;
            }
            catch(Exception x)
            {
                index = 1;
                listLevelHammer.Add(new HammerColor());
                listLevelHammer[listLevelHammer.Count - 1].colorType = new List<ColorType>(tempListHammerColor);
                tempListHammerColor.Clear();
                continue;
            }
        }
    }
    #endregion

    #region Read Layout Data
    public void ReadLayoutData()
    {
        tempLayoutData = levelLayoutDatas.text.Split(new string[] { "", " ", ",", "\n", ('"').ToString(), LINE_SPLIT_RE }, StringSplitOptions.RemoveEmptyEntries);

        scaleDatas.Clear(); //Reset
        positionDatas.Clear(); //Reset
        hammerPositionDatas.Clear(); //Reset

        int index = 1;
        Vector3 tempScale = Vector3.zero;
        Vector3 tempPosition = Vector3.zero;
        for (int i = 0; i < tempLayoutData.Length; i++)
        {
            try
            {
                if (index == 1) tempScale.x = float.Parse(tempLayoutData[i]);
                else if (index == 2) tempScale.y = float.Parse(tempLayoutData[i]);
                else if (index == 3) tempScale.z = float.Parse(tempLayoutData[i]);
                else if (index == 4) tempPosition.x = float.Parse(tempLayoutData[i]);
                else if (index == 5) tempPosition.y = float.Parse(tempLayoutData[i]);
                else if (index == 6) tempPosition.z = float.Parse(tempLayoutData[i]);
                else if (index == 7)
                {
                    scaleDatas.Add(tempScale);
                    positionDatas.Add(tempPosition);
                    hammerPositionDatas.Add(float.Parse(tempLayoutData[i]));
                }

                index++;
                if (index == 8) index = 1;
            }
            catch (Exception x)
            {
                index = 1;
            }
        }
    }
    #endregion

    #region Read CSV file
    private void CSVReader(int levelID)
    {
        listBrickIDColorID.Clear();
        colorList.Clear();
        listAllColorID.Clear();
        mapSize = Vector2Int.zero;

        //Read map data
        string[] tempData = dataText[levelID].text.Split(new string[] { "", " ", ",", "\n", ('"').ToString(), LINE_SPLIT_RE }, StringSplitOptions.RemoveEmptyEntries);
        string[] lineCount = dataText[levelID].text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);    //Là số lần xuống dòng (Dùng để đếm số hàng của file excell)

        mapSize = new Vector2Int((tempData.Length / lineCount.Length - 1) / 3, lineCount.Length); //Là size của map
        int index = 1;
        for (int j = 0; j < tempData.Length; j++)
        {
            if (j == mapSize.x * 3 * index + index - 1)    //TODO: Loại bỏ những phần tử rỗng trong tempData
            {
                index++;
            }
            else if (tempData[j] != "" && tempData[j] != null)
            {
                listAllColorID.Add(int.Parse(tempData[j]));   //List tất cả mã màu trong CSV (Tất cả các số trong file CSV)
            }
        }
        for(int i = 0; i < mapSize.y; i++)    //Tạo Dictionary chứa Brick ID và màu rgb.
        {
            Vector3Int tempVector3int;
            for(int j = 0; j < mapSize.x; j++)
            {
                tempVector3int = new Vector3Int(listAllColorID[(i * mapSize.x + j) * 3], listAllColorID[(i * mapSize.x + j) * 3 + 1], listAllColorID[(i * mapSize.x + j) * 3 + 2]);   //Gán mã ID của color cho biến tạm
                listBrickIDColorID.Add(new Vector2Int(j, mapSize.y-i-1), tempVector3int); //Add từng colorID theo key là BrickID
                if (!colorList.Contains(tempVector3int)) colorList.Add(tempVector3int);
            }
        }
        
        CreateSOData(levelID);
    }
    #endregion

    private void CreateSOData(int index)
    {
        tempBrickDataList.Clear();
        levelData = ScriptableObjectUtility.CreateAsset<LevelData>("new_"+(index+1));
        levelData.gameMode = GameMode.Normal;
        for (int i = 0; i < colorList.Count; i++)
        {
            if (FindColorType(colorList[i]) == ColorType.Clear) continue;
            levelData.brickDatas.Add(new BrickData());
            levelData.brickDatas[levelData.brickDatas.Count-1].colorType = FindColorType(colorList[i]);
        }

        foreach (KeyValuePair<Vector2Int, Vector3Int> item in listBrickIDColorID)   //Chia thành list các màu riêng biệt rồi tách các mảng màu đó ra
        {
            for (int i = 0; i < levelData.brickDatas.Count; i++)
            {
                if (levelData.brickDatas[i].colorType == FindColorType(item.Value))
                {
                    levelData.brickDatas[i].listBricksID.Add(item.Key);
                }
            }
        }

        levelData = SplitBrick(levelData, false);
        //Add Hammer
        levelData.hammerColor = new List<ColorType>(listLevelHammer[index].colorType);

        //Add Layout Datas
        levelData.mapScale = scaleDatas[index];
        levelData.mapPosition = positionDatas[index];
        levelData.hammerPosition = hammerPositionDatas[index];

        //Add Map size
        levelData.mapSize = mapSize;
        EditorUtility.SetDirty(levelData);
    }

    public LevelData SplitBrick(LevelData levelData, bool isEditor) //isEditor để đánh dấu xem đang sinh ra data mới theo cách nào. True: Đang sửa trong game. False: Sinh ra từ file CSV
    {
        tempBrickDataList.Clear();
        int a = 0;
        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            a += levelData.brickDatas[i].listBricksID.Count;
        }
        
        int tempNumber = 0;
        tempNumber = (levelData.brickDatas.Count);
        
        for (int i = 0; i < tempNumber; i++)    //Xét từng BrickData
        {
            int CountingNum = 0;
            while (levelData.brickDatas[i].listBricksID.Count > 1)
            {
                tempBrickData.listBricksID.Clear();
                tempBrickData.listBricksID.Add(levelData.brickDatas[i].listBricksID[0]);
                for (int k = 0; k < tempBrickData.listBricksID.Count; k++)
                {
                    for (int j = 0; j < levelData.brickDatas[i].listBricksID.Count; j++)
                    {
                        if (Vector2Int.Distance(levelData.brickDatas[i].listBricksID[j], tempBrickData.listBricksID[k]) < 1.2f &&
                            Vector2Int.Distance(levelData.brickDatas[i].listBricksID[j], tempBrickData.listBricksID[k]) > 0.5f &&
                            !(tempBrickData.listBricksID[k].x / mapSize.x == 0 && levelData.brickDatas[i].listBricksID[j].x / mapSize.x == 1))
                        {
                            if (!tempBrickData.listBricksID.Contains(levelData.brickDatas[i].listBricksID[j])) tempBrickData.listBricksID.Add(levelData.brickDatas[i].listBricksID[j]);
                        }
                    }
                }
                if (tempBrickData.listBricksID.Count > 1)
                {
                    levelData.brickDatas.Add(new BrickData());
                    if (isEditor) levelData.brickDatas[levelData.brickDatas.Count - 1].colorType = listBrickIDColorType[tempBrickData.listBricksID[0]];
                    else levelData.brickDatas[levelData.brickDatas.Count - 1].colorType = FindColorType(listBrickIDColorID[tempBrickData.listBricksID[0]]);
                    int Counting = 0;
                    while (tempBrickData.listBricksID.Count > 0)
                    {
                        levelData.brickDatas[levelData.brickDatas.Count - 1].listBricksID.Add(tempBrickData.listBricksID[0]);
                        levelData.brickDatas[i].listBricksID.Remove(tempBrickData.listBricksID[0]);
                        tempBrickData.listBricksID.Remove(tempBrickData.listBricksID[0]);
                        Counting++;
                        if (Counting > 100000) break;
                    }
                }
                CountingNum++;
                if (CountingNum > 100000) break;
            }
        }

        //Clear empty list
        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            if (levelData.brickDatas[i].listBricksID.Count > 1)
            {
                tempBrickDataList.Add(levelData.brickDatas[i]);
            }
        }
        levelData.brickDatas.Clear();
        for (int i = 0; i < tempBrickDataList.Count; i++)
        {
            levelData.brickDatas.Add(tempBrickDataList[i]);
        }

        return levelData;
    }

    #region Convert ColorID to ColorType
    private ColorType FindColorType(Vector3Int colorID)
    {
        if (colorID == new Vector3Int(254, 254, 254)) return ColorType.White;
        if (colorID == new Vector3Int(0, 0, 0)) return ColorType.Black;

        if(colorID == new Vector3Int(152, 0, 0)) return ColorType.RedBerry_1;
        if (colorID == new Vector3Int(230, 184, 175)) return ColorType.RedBerry_2;
        if (colorID == new Vector3Int(221, 126, 107)) return ColorType.RedBerry_3;
        if (colorID == new Vector3Int(204, 65, 37)) return ColorType.RedBerry_4;
        if (colorID == new Vector3Int(166, 28, 0)) return ColorType.RedBerry_5;
        if (colorID == new Vector3Int(133, 32, 12)) return ColorType.RedBerry_6;

        if (colorID == new Vector3Int(255, 0, 0)) return ColorType.Red_1;
        if (colorID == new Vector3Int(244, 204, 204)) return ColorType.Red_2;
        if (colorID == new Vector3Int(234, 153, 153)) return ColorType.Red_3;
        if (colorID == new Vector3Int(224, 102, 102)) return ColorType.Red_4;
        if (colorID == new Vector3Int(204, 0, 0)) return ColorType.Red_5;
        if (colorID == new Vector3Int(153, 0, 0)) return ColorType.Red_6;

        if (colorID == new Vector3Int(255, 153, 0)) return ColorType.Orange_1;
        if (colorID == new Vector3Int(252, 229, 205)) return ColorType.Orange_2;
        if (colorID == new Vector3Int(249, 203, 156)) return ColorType.Orange_3;
        if (colorID == new Vector3Int(246, 178, 107)) return ColorType.Orange_4;
        if (colorID == new Vector3Int(230, 145, 56)) return ColorType.Orange_5;
        if (colorID == new Vector3Int(180, 95, 6)) return ColorType.Orange_6;

        if (colorID == new Vector3Int(255, 255, 0)) return ColorType.Yellow_1;
        if (colorID == new Vector3Int(255, 242, 204)) return ColorType.Yellow_2;
        if (colorID == new Vector3Int(255, 229, 153)) return ColorType.Yellow_3;
        if (colorID == new Vector3Int(255, 217, 102)) return ColorType.Yellow_4;
        if (colorID == new Vector3Int(241, 194, 50)) return ColorType.Yellow_5;
        if (colorID == new Vector3Int(191, 144, 0)) return ColorType.Yellow_6;

        if (colorID == new Vector3Int(0, 255, 0)) return ColorType.Green_1;
        if (colorID == new Vector3Int(217, 234, 211)) return ColorType.Green_2;
        if (colorID == new Vector3Int(182, 215, 168)) return ColorType.Green_3;
        if (colorID == new Vector3Int(147, 196, 125)) return ColorType.Green_4;
        if (colorID == new Vector3Int(106, 168, 79)) return ColorType.Green_5;
        if (colorID == new Vector3Int(56, 118, 29)) return ColorType.Green_6;

        if (colorID == new Vector3Int(0, 255, 255)) return ColorType.Cyan_1;
        if (colorID == new Vector3Int(208, 224, 227)) return ColorType.Cyan_2;
        if (colorID == new Vector3Int(162, 196, 201)) return ColorType.Cyan_3;
        if (colorID == new Vector3Int(118, 165, 175)) return ColorType.Cyan_4;
        if (colorID == new Vector3Int(69, 129, 142)) return ColorType.Cyan_5;
        if (colorID == new Vector3Int(19, 79, 92)) return ColorType.Cyan_6;

        if (colorID == new Vector3Int(74, 134, 232)) return ColorType.Blue_1;
        if (colorID == new Vector3Int(208, 224, 227)) return ColorType.Blue_2;
        if (colorID == new Vector3Int(164, 194, 244)) return ColorType.Blue_3;
        if (colorID == new Vector3Int(109, 158, 235)) return ColorType.Blue_4;
        if (colorID == new Vector3Int(60, 120, 216)) return ColorType.Blue_5;
        if (colorID == new Vector3Int(17, 85, 204)) return ColorType.Blue_6;

        if (colorID == new Vector3Int(0, 0, 255)) return ColorType.SeaBlue_1;
        if (colorID == new Vector3Int(207, 226, 243)) return ColorType.SeaBlue_2;
        if (colorID == new Vector3Int(164, 194, 244)) return ColorType.SeaBlue_3;
        if (colorID == new Vector3Int(109, 158, 235)) return ColorType.SeaBlue_4;
        if (colorID == new Vector3Int(61, 133, 198)) return ColorType.SeaBlue_5;
        if (colorID == new Vector3Int(11, 83, 148)) return ColorType.SeaBlue_6;

        if (colorID == new Vector3Int(153, 0, 255)) return ColorType.Purple_1;
        if (colorID == new Vector3Int(217, 210, 233)) return ColorType.Purple_2;
        if (colorID == new Vector3Int(180, 167, 214)) return ColorType.Purple_3;
        if (colorID == new Vector3Int(142, 124, 195)) return ColorType.Purple_4;
        if (colorID == new Vector3Int(103, 78, 167)) return ColorType.Purple_5;
        if (colorID == new Vector3Int(53, 28, 117)) return ColorType.Purple_6;

        if (colorID == new Vector3Int(255, 0, 255)) return ColorType.Magenta_1;
        if (colorID == new Vector3Int(234, 209, 220)) return ColorType.Magenta_2;
        if (colorID == new Vector3Int(213, 166, 189)) return ColorType.Magenta_3;
        if (colorID == new Vector3Int(194, 123, 160)) return ColorType.Magenta_4;
        if (colorID == new Vector3Int(166, 77, 121)) return ColorType.Magenta_5;
        if (colorID == new Vector3Int(116, 27, 71)) return ColorType.Magenta_6;

        if (colorID == new Vector3Int(243, 243, 243)) return ColorType.Grey_1;
        if (colorID == new Vector3Int(215, 215, 215)) return ColorType.Grey_2;
        if (colorID == new Vector3Int(200, 200, 200)) return ColorType.Grey_3;
        if (colorID == new Vector3Int(160, 160, 160)) return ColorType.Grey_4;
        if (colorID == new Vector3Int(120, 120, 120)) return ColorType.Grey_5;
        if (colorID == new Vector3Int(100, 100, 100)) return ColorType.Grey_6;
        if (colorID == new Vector3Int(70, 70, 70)) return ColorType.Grey_7;
        if (colorID == new Vector3Int(50, 50, 50)) return ColorType.Grey_8;

        return ColorType.Clear;
    }
    #endregion

    public void Reset()
    {
        dataText.Clear();
        listAllColorID.Clear();
        listBrickIDColorID.Clear();
        colorList.Clear();
        tempBrickDataList.Clear();
        listLevelHammer.Clear();
        tempHammerData = new string[0];
    }

    #region Editor
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit,rayCastDistance))
            {
                if (LevelManager.Instance.brickManager.SearchBrickClosest(hit.point) == null) return;
                //Brick brick = LevelManager.Instance.brickManager.SearchBrickClosest(hit.point);
                SellectingBrick = LevelManager.Instance.brickManager.SearchBrickClosest(hit.point);
                SellectingBrick.ChangeColor(UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).CurrentColor());
                if (listBrickIDColorType.ContainsKey(SellectingBrick.id)) listBrickIDColorType.Remove(SellectingBrick.id);
                listBrickIDColorType.Add(SellectingBrick.id, UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).CurrentColor());
            }
        }
    }
    #endregion
#endif
}

[System.Serializable]
public class HammerColor
{
    public List<ColorType> colorType;
}
