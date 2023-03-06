using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BonusLevelManager : Singleton<BonusLevelManager>
{
    const string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("---------------Manager---------------")]
    public Dictionary<Vector3Int, Material> rgbID2Material = new Dictionary<Vector3Int, Material>();        //list chứa Material theo key là ColorID. List này sẽ được thay đổi khi load từng level mới
    public Dictionary<Vector3Int, Material> rgbID2SampleMaterial = new Dictionary<Vector3Int, Material>();  //list chứa Material của sample theo key là ColorID. List này sẽ được thay đổi khi load từng level mới
    private Vector2Int bonusLevelMapSize;   //Được xác định dư
    public BonusMaterialData materialsData;
    public BonusMaterialData sampleMaterialsData;
    public BonusMaterialData defaultMaterialsData;
    public int playingLevelID;

    [Header("---------------Read CSV files---------------")]
    public Dictionary<Vector2Int, Vector3Int> listBrickIDColorID = new Dictionary<Vector2Int, Vector3Int>();//Vector2Int là tọa độ brick, Vector3Int là mã màu rgb
    public List<TextAsset> levelDataTextAssets; //File CSV
    public List<int> listAllColorID;
    public List<Vector3Int> colorList = new List<Vector3Int>();

    [Header("---------------Camera---------------")]
    public Transform marginTransform;    //Điểm Margin dùng để điều chỉnh tầm nhìn của camera. 
    public PaintLevelLayout paintLevelLayout;

    private void Awake() //Load CSV files
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
        playingLevelID = 0;
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        //DataManager.Instance.LoadData();
        if (DataManager.Instance.gameData.bonusDesireLevel != -1)
        {
            LoadLevel((DataManager.Instance.gameData.bonusDesireLevel - 1) % levelDataTextAssets.Count);
            playingLevelID = DataManager.Instance.gameData.bonusDesireLevel;
        }
        else 
        {
            LoadLevel((DataManager.Instance.gameData.currentPaintLevel - 1) % levelDataTextAssets.Count);
            playingLevelID = DataManager.Instance.gameData.currentPaintLevel;
        }
        GameManager.Instance.gameState = GameState.GamePlay;
        if (DataManager.Instance.gameData.paintLevelInformation[playingLevelID-1].isFirstTime)
        {
            DataManager.Instance.gameData.paintLevelInformation[playingLevelID-1].isFirstTime = false;
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_bonus_start_ + playingLevelID.ToString(), Parameter.level, playingLevelID.ToString());
            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_start_ + ((DataManager.Instance.gameData.currentLevel > 9) ? DataManager.Instance.gameData.currentLevel.ToString() : ("0" + DataManager.Instance.gameData.currentLevel.ToString())), DataManager.Instance.gameData.currentLevel.ToString(), levelType.paint);
        }
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_level_start, Parameter.level, (DataManager.Instance.gameData.currentLevel).ToString());
        DataManager.Instance.gameData.levels_played++;
        DataManager.Instance.SaveGame();
        DataManager.Instance.gameData.paintLevelInformation[playingLevelID - 1].isUnlock = true;
        GameManager.Instance.goHomeMenu = true;
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.V))   //Nhấn S+V để save vị trí của layout trong game
        {
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].levelID = DataManager.Instance.gameData.currentPaintLevel;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].cameraPos = Camera.main.transform.position;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].brickPos = BonusBrickManager.Instance.tf.position;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].brickScale = BonusBrickManager.Instance.tf.localScale;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].sampleBrickPos = SampleBrickScript.Instance.tf.position;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].sampleBrickScale = SampleBrickScript.Instance.tf.localScale;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].sampleBrickBGScale = SampleBrickScript.Instance.sampleBackGround.localScale;
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].sampleBrickBGPos = SampleBrickScript.Instance.sampleBackGround.localPosition;
            Debug.Log("Save paint layout level:" + (DataManager.Instance.gameData.currentPaintLevel));
            EditorUtility.SetDirty(paintLevelLayout);
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.E)) //Nhấn S+E để lưu vị trí của camera lúc endgame
        {
            paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].cameraEndGamePos = Camera.main.transform.position;
            Debug.Log("Save Endgame camera pos level:" + (DataManager.Instance.gameData.currentPaintLevel));
            EditorUtility.SetDirty(paintLevelLayout);
        }
    }
#endif

    public void SetupLayout()
    {
        BonusBrickManager.Instance.tf.position = paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel - 1)%BonusLevelManager.Instance.levelDataTextAssets.Count].brickPos;
        BonusBrickManager.Instance.tf.localScale = paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel - 1) % BonusLevelManager.Instance.levelDataTextAssets.Count].brickScale;
        SampleBrickScript.Instance.tf.position = paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel - 1) % BonusLevelManager.Instance.levelDataTextAssets.Count].sampleBrickPos;
        SampleBrickScript.Instance.sampleBackGround.localScale = paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel - 1) % BonusLevelManager.Instance.levelDataTextAssets.Count].sampleBrickBGScale;
        SampleBrickScript.Instance.sampleBackGround.localPosition = paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel - 1) % BonusLevelManager.Instance.levelDataTextAssets.Count].sampleBrickBGPos;
    }

    public void LoadLevel(int levelID)
    {
        CSVReader(levelID);
        rgbID2Material.Clear();
        rgbID2SampleMaterial.Clear();
        for (int i = 0; i < colorList.Count; i++)  //Mỗi lần load level mới thì thay đổi lại material để sử dụng cho level đó
        {
            materialsData.listMaterials[i].color = new Color32((byte)colorList[i].x, (byte)colorList[i].y, (byte)colorList[i].z, 255);
            sampleMaterialsData.listMaterials[i].color = new Color32((byte)colorList[i].x, (byte)colorList[i].y, (byte)colorList[i].z, 255);
            rgbID2Material.Add(new Vector3Int(colorList[i].x, colorList[i].y, colorList[i].z), materialsData.listMaterials[i]);
            rgbID2SampleMaterial.Add(new Vector3Int(colorList[i].x, colorList[i].y, colorList[i].z), sampleMaterialsData.listMaterials[i]);
        }
        BonusBrickManager.Instance.mapSize = bonusLevelMapSize;
        BonusBrickManager.Instance.SpawnBrick();
        UI_Game.Instance.OpenUI(UIID.BonusGamePlay);
    }

    public void LoadNextLevel() //Mở normal level tiếp theo
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.MainSceneStr);
    }

    public void ResetLevel() //Khi Reset level thì chỉ cho màu của Brick về default
    {
        BonusBrickManager.Instance.ReloadLevelBrick();
    }

    #region Read CSV file
    private void CSVReader(int levelDataID)
    {
        listAllColorID.Clear();     //Reset Data
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
}
