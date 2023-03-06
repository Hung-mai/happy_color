using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class BonusLevelCSVReader : Singleton<BonusLevelCSVReader>
{
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private List<TextAsset> levelDataTextAssets; //File CSV
    private List<int> listAllColorID;
    private Dictionary<Vector2Int, Vector3Int> listBrickIDColorID = new Dictionary<Vector2Int, Vector3Int>();    //Vector2Int là tọa độ brick, Vector3Int là mã màu rgb
    private List<Vector3Int> colorList = new List<Vector3Int>();
    private Vector2Int bonusLevelMapSize;
    private void Awake()
    {
        int index = 1;
        TextAsset textAsset = null;
        while (true)
        {
            textAsset = Resources.Load<TextAsset>("BonusLevel_" + index);
            if (textAsset == null) return;
            levelDataTextAssets.Add(textAsset);
            index++;
        }
    }

    #region Read CSV file
    private void CSVReader(int levelDataID)
    {
        listAllColorID.Clear(); //Reset Data
        listBrickIDColorID.Clear();
        colorList.Clear();
        bonusLevelMapSize = Vector2Int.zero;
        string[] tempData = levelDataTextAssets[levelDataID].text.Split(new string[] { "", " ", ",", "\n", ('"').ToString(), LINE_SPLIT_RE }, StringSplitOptions.RemoveEmptyEntries);
        string[] lineCount = levelDataTextAssets[levelDataID].text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);    //Là số lần xuống dòng (Dùng để đếm số hàng của file excell)
        bonusLevelMapSize = new Vector2Int((tempData.Length / lineCount.Length - 1) / 3, lineCount.Length); //Là size của map
        int index = 1;
        for (int j = 0; j < tempData.Length; j++)
        {
            if (j == bonusLevelMapSize.x * 3 * index + index - 1)    //TODO: Loại bỏ những phần tử rỗng trong tempData
            {
                index++;
            }
            else if (tempData[j] != "" && tempData[j] != null)
            {
                listAllColorID.Add(int.Parse(tempData[j]));   //List tất cả mã màu trong CSV
            }
        }
        for (int i = 0; i < bonusLevelMapSize.y; i++)    //Tạo Dictionary chứa Brick ID và màu rgb.
        {
            Vector3Int tempVector3int;
            for (int j = 0; j < bonusLevelMapSize.x; j++)
            {
                tempVector3int = new Vector3Int(listAllColorID[(i * bonusLevelMapSize.x + j) * 3], listAllColorID[(i * bonusLevelMapSize.x + j) * 3 + 1], listAllColorID[(i * bonusLevelMapSize.x + j) * 3 + 2]);   //Gán mã ID của color cho biến tạm
                listBrickIDColorID.Add(new Vector2Int(j, i), tempVector3int); //Add từng colorID theo key là BrickID
                if (!colorList.Contains(tempVector3int)) colorList.Add(tempVector3int);
            }
        }
    }
    #endregion

    public int GetNumberOfLevel()
    {
        return levelDataTextAssets.Count;
    }

    public List<Vector3Int> GetColorList()
    {
        return colorList;
    }

    public Vector2Int GetLevelSize()
    {
        return bonusLevelMapSize;
    }

    public Dictionary<Vector2Int, Vector3Int> rgbID2ColorID()
    {
        return listBrickIDColorID;
    }

    public void LoadLevelData(int levelID)  //Lấy dữ liệu từ file CSV rồi đẩy vào file BonusLevelData SO
    {
        CSVReader(levelID);
    }
}
