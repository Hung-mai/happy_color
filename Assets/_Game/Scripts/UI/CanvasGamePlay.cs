using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasGamePlay : UICanvas
{
    public const string MOVEOUT = "moveout";
    public const string MOVEHAMMERTOCENTER = "MoveHammerToCenter";
    public const string LEVEL = "LEVEL ";
    //public UserData userData;

    public TextMeshProUGUI txt_timeSpecial;
    public GameObject btn_specialOffer;

    public Text levelTxt;
    public GameObject IncorrectWarning;
    public GameObject btnHideUI;

    //Build MKT
    public GameObject btnSetting;
    public GameObject btnSpecialLevel;
    public GameObject levelText;
    public GameObject btnReplay;
    public GameObject btnSkip;
    public GameObject progressgob;

    //Tutorial
    public GameObject tutorialPanel;
    public GameObject BGImage;
    public GameObject hand;

    public float timeCounting = 0;

    public bool checkKnock = false; //Kiểm tra xem player click vào bên trái hay bên phải màn hình trong tutorial

    //Move Hammers
    public RectTransform BtnMoveHammerTransform;
    public GameObject btnMoveRight;
    public GameObject btnMoveLeft;
    public Animator btnMoveAnim;

    //Progress
    public Image[] progressImage;
    public Sprite[] sampleSprite; //1: NextLevel, 2: CurrentLevel, 3: PassLevel
    public Image currentThemeImage;
    public Image targetThemeImage;
    public Sprite[] themeSpriteList;
    public bool isAllowToReplay = false;

    //Editor
    public GameObject mainPanel;
    public GameObject editorPanel;
    public Text levelTextInput;
    public Text sellectingHammerText;
    public GameObject hammerBtnParent;
    public GameObject colorBtnParent;
    private int sellectingHammer;
    public List<ColorType> hammerColorList;
    public LevelData levelData;
    public GameObject hideEditorUIBtn;
    public GameObject showMainUIBtn;
    public Slider slider;

    //Safe Screen mode
    public RectTransform[] safeScreenModegob;

    private void Awake()
    {
        SetupSafeScreen();
    }
    private void OnEnable()
    {
        if (GameManager.Instance.IsEditor) return;
        timeCounting = 0;
        if (DataManager.Instance.gameData.currentLevel == 1 || DataManager.Instance.gameData.currentLevel == 2) ShowTutorial();
        else
        {
            btnSkip.SetActive(true);
            tutorialPanel.SetActive(false);
        }
        BtnMoveHammerTransform.gameObject.SetActive(false);

        if(DataManager.Instance.gameData.specialShowing == false)
        {
            btn_specialOffer.SetActive(false);
        }
        else
        {
            int timeCount = GameManager.Instance.timeCountSpecial;

            string hours = ((timeCount / 60) / 60).ToString();
            string minutes = timeCount / 60 < 10 ? "0" + (timeCount / 60).ToString() : (timeCount / 60).ToString();
            string seconds = timeCount % 60 < 10 ? "0" + (timeCount % 60).ToString() : (timeCount % 60).ToString();
            string time = hours + ":" + minutes + ":" + seconds;
            txt_timeSpecial.text = time;
            btn_specialOffer.SetActive(true);
        }
        Timer.Schedule(this, 0.8f, () => { LevelManager.Instance.CheckSpecialoffer(); });
    }

    public void Btn_ShowSpecialOffer()
    {
        UI_Game.Instance.OpenUI(UIID.SpecialOffer);
    }


    private void Update()
    {
        timeCounting += Time.deltaTime;
    }
    public override void Setup()
    {
        base.Setup();
        if (GameManager.Instance.IsEditor) 
        {
            mainPanel.SetActive(false);
            editorPanel.SetActive(true);
#if UNITY_EDITOR
            EditorInit();
#endif
            colorBtnParent.SetActive(false);
            hideEditorUIBtn.SetActive(true);
            showMainUIBtn.SetActive(true);
            LoadMainGameData.Instance.gameObject.SetActive(true);
            return;
        }
        else
        {
            mainPanel.SetActive(true);
            editorPanel.SetActive(false);
            hideEditorUIBtn.SetActive(false);
            showMainUIBtn.SetActive(false);
            LoadMainGameData.Instance.gameObject.SetActive(false);
        }
        UpdateBtnMoveHammers();
        IncorrectWarning.SetActive(false);
        btnHideUI.SetActive(GameManager.Instance.IsBuildMaketing);
        levelTxt.text = LEVEL + (DataManager.Instance.gameData.currentLevel);
        FirebaseManager.Instance.OnSetUserProperty();
        UpdateProgress();
        SetupThemeImage();
    }

    public override void Close()
    {
        base.Close();
        RandomNextTheme();
    }
    void SetupSafeScreen()
    {
        if (Screen.height != Screen.safeArea.height)
        {
            for (int i = 0; i < safeScreenModegob.Length; i++)
            {
                Vector3 temp = safeScreenModegob[i].position;
                temp.y -= (Screen.height - Screen.safeArea.height);
                safeScreenModegob[i].position = temp;
            }
        }
    }

    public void SetupThemeImage()
    {
        currentThemeImage.sprite = themeSpriteList[DataManager.Instance.gameData.currentThemeID % themeSpriteList.Length];
        targetThemeImage.sprite = themeSpriteList[DataManager.Instance.gameData.targetThemeID % themeSpriteList.Length];
    }

    #region Button
    public void Btn_Replay()
    {
        if (Control.Instance.isCheckWave || GameManager.Instance.gameState == GameState.BlockAction || isAllowToReplay) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetInterPlacement(placement.Btn_replay_ingame);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
        LevelManager.Instance.OnReplay();
        Close();
    }

    public void Btn_Skip()
    {
        if (Control.Instance.isCheckWave || GameManager.Instance.gameState == GameState.BlockAction || isAllowToReplay) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetRewardPlacement(placement.Level_skip);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_level_click, Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
        FirebaseManager.Instance.LogLevelComplete(EventName.g_level_complete.ToString(), (DataManager.Instance.gameData.currentLevel).ToString(), Control.Instance.timeCounting);
        Control.Instance.timeCounting = 0;
        AppLovinController.instance.ShowRewardedAd(()=> 
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_end_ + DataManager.Instance.gameData.currentLevel.ToString(), Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());

            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_end_ + (((DataManager.Instance.gameData.currentLevel) > 9) ? (DataManager.Instance.gameData.currentLevel).ToString() : ("0" + (DataManager.Instance.gameData.currentLevel).ToString())), (DataManager.Instance.gameData.currentLevel).ToString(), levelType.normal);

            DataManager.Instance.gameData.currentLevel++;
            DataManager.Instance.gameData.allLevelCounting++;
            DataManager.Instance.SaveGame();
            LoadNextLevel();
        });
    }

    public void LoadNextLevel()
    {
        LevelManager.Instance.LoadLevel();
        DataManager.Instance.SaveGame();
        Close();
        UI_Game.Instance.OpenUI(UIID.GamePlay);
        HammerControl.Instance.MoveToPlayGame();
    }

    public void SettingButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!GameManager.Instance.IsState(GameState.BlockAction) && !GameManager.Instance.IsState(GameState.Pause))
        {
            UI_Game.Instance.OpenUI(UIID.Setting);
        }
    }

    public void SellectGiftBtn()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.Property);
    }

    public void BtnBonusLevelList()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.BonusLevelList);
    }

    public void StartWarning()
    {
        IncorrectWarning.SetActive(true);
        Timer.Schedule(this, 1f, () => { IncorrectWarning.SetActive(false); });
    }

    public void BtnHideUI()
    {
        levelText.SetActive(!btnSetting.activeSelf);
        btnSpecialLevel.SetActive(!btnSetting.activeSelf);
        btnReplay.SetActive(!btnSetting.activeSelf);
        btnSkip.SetActive(!btnSetting.activeSelf);
        progressgob.SetActive(!btnSetting.activeSelf);
        btnSetting.SetActive(!btnSetting.activeSelf);
    }
#endregion

    #region Tutorial
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        BGImage.SetActive(false);
        hand.SetActive(false);
        btnSkip.SetActive(false);

        BGImage.SetActive(true);
        
        if (DataManager.Instance.gameData.currentLevel == 1) 
        {
            LevelManager.Instance?.UnEnableToClickOnBrick();
            StartCoroutine(CheckTheTap());
        }
        if (DataManager.Instance.gameData.currentLevel == 2 && timeCounting < 1f) 
        {
            StartCoroutine(CheckTheTapLevel2());
            BGImage.SetActive(false);
            hand.SetActive(false);
        } 
    }
    #region Tutorial Level 1
    IEnumerator CheckTheTap()
    {
        yield return null;
        if (GameManager.Instance.gameState == GameState.BlockAction) hand.SetActive(false);
        try
        {
            if (HammerControl.Instance.hammers.Count < 3)
            {
                hand.SetActive(false);
            }
            if(Control.Instance.isCheckWave|| Control.Instance.isWinWave) hand.SetActive(false);
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    BGImage.SetActive(false);
                    LevelManager.Instance?.EnableToClickOnBrick();
                    hand.SetActive(true);
                    if (HammerControl.Instance.currentHammer == null) hand.GetComponent<RectTransform>().position = new Vector3(Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).x * Screen.width - 35f, Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).y * Screen.height - 35f, 0);
                }
                else if (HammerControl.Instance.currentHammer == HammerControl.Instance.hammers[0] || HammerControl.Instance.currentHammer == HammerControl.Instance.hammers[2]) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[423].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[423].tf.position).y * Screen.height, 0), 1f);
                else if (HammerControl.Instance.currentHammer == HammerControl.Instance.hammers[1]) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[434].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[434].tf.position).y * Screen.height, 0), 1f);
                StartCoroutine(CheckTheTap());
            }
            
        }catch(Exception c)
        {

        }
    }
    #endregion
    #region Tutorial Level 2
    IEnumerator CheckTheTapLevel2()
    {
        yield return null;
        if (GameManager.Instance.gameState == GameState.BlockAction)
        {
            timeCounting = 0;
            hand.SetActive(false);
        }
        try
        {
            if (HammerControl.Instance.currentHammer == null)
            {
                if(HammerControl.Instance.hammers.Count == 3)
                {
                    hand.GetComponent<RectTransform>().position = new Vector3(Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[HammerControl.Instance.hammers.Count - 1].tf.position).x * Screen.width - 35f, Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).y * Screen.height - 35f, 0);
                    if (timeCounting > 3f) hand.SetActive(true);
                }
                else
                {
                    hand.GetComponent<RectTransform>().position = new Vector3(Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).x * Screen.width - 35f, Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[1].tf.position).y * Screen.height - 35f, 0);
                    if (timeCounting > 3f) hand.SetActive(true);
                }
                
            }
            else
            {
                if (timeCounting > 3f) hand.SetActive(true);
                if (HammerControl.Instance.currentHammer.ColorType == ColorType.Yellow_1)
                {
                    if (BrickManager.Instance.totalBricks[294].ColorType != ColorType.Yellow_1) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).y * Screen.height, 0), 1f);
                    //else if (BrickManager.Instance.totalBricks[149].ColorType != ColorType.Yellow_1) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[149].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[149].tf.position).y * Screen.height, 0), 1f);
                    else hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[149].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[149].tf.position).y * Screen.height, 0), 1f);
                }
                else
                {
                    if (BrickManager.Instance.totalBricks[294].ColorType != ColorType.Yellow_1) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).y * Screen.height, 0), 1f);
                    else if (BrickManager.Instance.totalBricks[294].ColorType != ColorType.Yellow_1) hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).y * Screen.height, 0), 1f);
                    else hand.GetComponent<RectTransform>().DOMove(new Vector3(Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).x * Screen.width, Camera.main.WorldToViewportPoint(BrickManager.Instance.totalBricks[294].tf.position).y * Screen.height, 0), 1f);
                }
            }
        }
        catch (Exception)
        {

        }
        StartCoroutine(CheckTheTapLevel2());
    }
    #endregion
    #endregion

    #region Hammers
    public void BtnMoveHammer(bool rightleft) //0: Right, 1: Left
    {
        if (GameManager.Instance.gameState == GameState.BlockAction)
        {
            timeCounting = 0;
            return;
        }
        if (timeCounting > 0.5f) 
        {
            HammerControl.Instance.MoveTheHammers(rightleft);
            timeCounting = 0;
        } 
    }

    public void UpdateBtnMoveHammers()
    {
        HammerControl.Instance.banktf.position = new Vector3(0, HammerControl.Instance.banktf.position.y, HammerControl.Instance.banktf.position.z);
        try
        {
            if (HammerControl.Instance.hammers.Count <= 8)
            {
                //Invoke(MOVEHAMMERTOCENTER, 1f);
                Timer.Schedule(this, 0.6f, () =>
                {
                    MoveHammerToCenter();
                });
                btnMoveLeft.SetActive(false);
                btnMoveRight.SetActive(false);
                return;
            }
            else
            {
                btnMoveLeft.SetActive(true);
                btnMoveRight.SetActive(true);
            }
            if (Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).x > 0.1f) btnMoveRight.SetActive(false);
            if (Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[HammerControl.Instance.hammers.Count - 1].tf.position).x < 0.95f) btnMoveLeft.SetActive(false);
            if (Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[0].tf.position).x > 0.5f) MoveHammerToCenter();
            if (Camera.main.WorldToViewportPoint(HammerControl.Instance.hammers[HammerControl.Instance.hammers.Count - 1].tf.position).x < 0.5f) MoveHammerToCenter();
        }
        catch (Exception c)
        {

        }
        
    }

    private void MoveHammerToCenter()
    {
        HammerControl.Instance.tf.DOMoveX(0, 0.5f);
        HammerControl.Instance.banktf.position = new Vector3(0, HammerControl.Instance.banktf.position.y, HammerControl.Instance.banktf.position.z);
    }

    public void SetBtnMoveHammers()
    {
        BtnMoveHammerTransform.position = new Vector3(Screen.width / 2, Camera.main.WorldToScreenPoint(HammerControl.Instance.tf.position).y- Screen.height/6.4f, 0);
        BtnMoveHammerTransform.gameObject.SetActive(true);
    }

    public void MoveBtn(bool moveDirection) //1: out, 0: in
    {
        btnMoveAnim.SetBool(MOVEOUT, moveDirection);
    }
    #endregion

    int levelIndex;
    int levelInProgressAmount;

    #region Progress
    public void UpdateProgress()
    {
        levelIndex = DataManager.Instance.gameData.currentLevel <= 5 ? DataManager.Instance.gameData.currentLevel-1 : (DataManager.Instance.gameData.currentLevel <= 12 ? (DataManager.Instance.gameData.currentLevel-6): (DataManager.Instance.gameData.currentLevel-3)%10); //Vị trí của level hiện tại trong 1 progress. 1 progress là từ cuối level paint này đến cuối level paint kia. 
        levelInProgressAmount = DataManager.Instance.gameData.currentLevel<=5?5: (DataManager.Instance.gameData.currentLevel <= 12 ?7:10);
        for (int i = 0; i < progressImage.Length; i++)
        {
            if(i >= levelInProgressAmount-1)
            {
                progressImage[i].gameObject.SetActive(false);
                continue;
            }else progressImage[i].gameObject.SetActive(true);
            if (i < levelIndex) progressImage[i].sprite = sampleSprite[2];
            else if (i == levelIndex) progressImage[i].sprite = sampleSprite[1];
            else progressImage[i].sprite = sampleSprite[0];
        }
    }

    public void RandomNextTheme()
    {
        if ((levelIndex == levelInProgressAmount-1))
        {
            DataManager.Instance.gameData.currentThemeID = DataManager.Instance.gameData.targetThemeID;
            if (DataManager.Instance.gameData.currentThemeID >= LevelManager.Instance.themeList.Length) DataManager.Instance.gameData.currentThemeID = 0;
            do
            {
                DataManager.Instance.gameData.targetThemeID = UnityEngine.Random.Range(0, 3);
            } while (DataManager.Instance.gameData.currentThemeID == DataManager.Instance.gameData.targetThemeID);
            DataManager.Instance.SaveGame();
        }
    }
    #endregion
#if UNITY_EDITOR
    #region Editor
    public void EditorInit()
    {
        for (int i = 4; i < colorBtnParent.transform.childCount; i++)
        {
            colorBtnParent.transform.GetChild(i-4).GetComponent<Image>().color = BrickManager.Instance.colorData.colorMats[i].color;
        }
        for (int i = 68; i < 71; i++)
        {
            colorBtnParent.transform.GetChild(i).GetComponent<Image>().color = BrickManager.Instance.colorData.colorMats[i-68].color;
        }
        LoadMainGameData.Instance.listBrickIDColorType.Clear();
    }

    public LevelData levelDataEditor = null;
    public void LoadLevelData()
    {
        LoadMainGameData.Instance.listBrickIDColorType.Clear();
        LoadMainGameData.Instance.listBrickIDColorType = new Dictionary<Vector2Int, ColorType>();
        BrickManager.Instance.SetData(LevelManager.Instance.levelDataBase);
        if (int.Parse(levelTextInput.text) > LevelManager.Instance.levelDatas.Count || int.Parse(levelTextInput.text) < 0) return;
        levelDataEditor = Resources.Load<LevelData>("LevelData_new_" + int.Parse(levelTextInput.text));
        BrickManager.Instance.SetData(levelDataEditor);
        BrickManager.Instance.tf.localScale = levelDataEditor.mapScale;
        BrickManager.Instance.tf.position = levelDataEditor.mapPosition;
        HammerControl.Instance.OnInit(levelDataEditor);
        HammerControl.Instance.tf.position = new Vector3(0, 0, -levelDataEditor.hammerPosition);
        hammerColorList = new List<ColorType>(levelDataEditor.hammerColor);
        UpdateHammerUI();
        for (int i = 0; i < levelDataEditor.brickDatas.Count; i++)
        {
            for (int j = 0; j < levelDataEditor.brickDatas[i].listBricksID.Count; j++)
            {
                LoadMainGameData.Instance.listBrickIDColorType.Add(levelDataEditor.brickDatas[i].listBricksID[j], levelDataEditor.brickDatas[i].colorType);
            }
        }
    }
    public void CreateLevelData(int isCreateNew) //0: Sửa từ level data cũ| 1: Tạo level data mới
    {
        levelData = null;
        if (isCreateNew == 1) levelData = ScriptableObjectUtility.CreateAsset<LevelData>(levelTextInput.text);
        else if (levelDataEditor != null) levelData = levelDataEditor;
        else return;
        levelData.gameMode = GameMode.Normal;

        //Find Size Map
        Vector2Int mapSize = new Vector2Int(0,0);
        
        foreach (KeyValuePair<Vector2Int, ColorType> item in LoadMainGameData.Instance.listBrickIDColorType)  
        {
            if (mapSize.x < item.Key.x) mapSize.x = item.Key.x;
            if (mapSize.y < item.Key.y) mapSize.y = item.Key.y;
        }
        mapSize.x++;
        mapSize.y++;
        LoadMainGameData.Instance.mapSize = mapSize;
        levelData.mapSize = mapSize;

        //HammerCollor
        levelData.hammerColor.Clear();
        for (int i = 0; i < hammerColorList.Count; i++)
        {
            if (hammerColorList[i]!=ColorType.Clear && hammerColorList[i] != ColorType.Editor)
            {
                levelData.hammerColor.Add(hammerColorList[i]);
            }
        }
        //levelData.hammerColor = new List<ColorType>(hammerColorList);

        //Map Scale 
        levelData.mapScale = BrickManager.Instance.tf.localScale;

        //BrickData
        List<ColorType> colorlist = new List<ColorType>();  //Tìm số lượng màu Level
        levelData.brickDatas.Clear();
        foreach (KeyValuePair<Vector2Int, ColorType> item in LoadMainGameData.Instance.listBrickIDColorType)
        {
            if (!colorlist.Contains(item.Value)) colorlist.Add(item.Value);
        }
        for (int i = 0; i < colorlist.Count; i++)
        {
            levelData.brickDatas.Add(new BrickData());
            levelData.brickDatas[levelData.brickDatas.Count - 1].colorType = colorlist[i];
        }

        foreach (KeyValuePair<Vector2Int, ColorType> item in LoadMainGameData.Instance.listBrickIDColorType)   //Chia thành list các màu riêng biệt rồi tách các mảng màu đó ra
        {
            for (int i = 0; i < levelData.brickDatas.Count; i++)
            {
                if (levelData.brickDatas[i].colorType == item.Value)
                {
                    levelData.brickDatas[i].listBricksID.Add(item.Key);
                }
            }
        }
        
        levelData = LoadMainGameData.Instance.SplitBrick(levelData, true);
        

        //Map Pos
        levelData.mapPosition = BrickManager.Instance.tf.position;

        //Hammer Pos
        levelData.hammerPosition = Mathf.Abs(HammerControl.Instance.tf.position.z);


        EditorUtility.SetDirty(levelData);
    }
    #endregion
#endif



    #region Button
    public void SellectHammerColor(int colorID) //From Panel
    {
        hammerBtnParent.transform.GetChild(sellectingHammer).GetComponent<Image>().color = BrickManager.Instance.colorData.colorMats[colorID].color;
        hammerColorList[sellectingHammer] = BrickManager.Instance.colorData.colorMats[colorID].colorType;
        colorBtnParent.SetActive(!colorBtnParent.activeSelf);
#if UNITY_EDITOR
        LoadMainGameData.Instance.rayCastDistance = colorBtnParent.activeSelf ? 0 : 1000;
#endif
        HammerClick(sellectingHammer);
    }

    public void HammerClick(int hammerID)
    {
        sellectingHammer = hammerID;
        sellectingHammerText.text = "Hammer: " + sellectingHammer+ " : " + hammerColorList[sellectingHammer].ToString();
    }

    public void SellectColorPanel()
    {
        colorBtnParent.SetActive(!colorBtnParent.activeSelf);
#if UNITY_EDITOR
        LoadMainGameData.Instance.rayCastDistance = colorBtnParent.activeSelf ? 0 : 1000;
#endif
    }

    public void SellectCamera(int cameraID)
    {
        if(cameraID == 1)
        {
            Camera.main.transform.position = new Vector3(0, 55.19f, -32.48f);
            Camera.main.transform.eulerAngles = new Vector3(63.75f, 0, 0);
        }
        else
        {
            Camera.main.transform.position = new Vector3(0, 100f*slider.value, 0);
            Camera.main.transform.eulerAngles = new Vector3(90f, 0, 0);
        }
    }

    public ColorType CurrentColor()
    {
        return hammerColorList[sellectingHammer];
    }

    public void HideUIEditor()
    {
        editorPanel.SetActive(!editorPanel.activeSelf);
    }

    public void ShowMainUI()
    {
        mainPanel.SetActive(!mainPanel.activeSelf);
    }

    

    public void AddNewHammer()
    {
        hammerColorList.Add(ColorType.Editor);
        UpdateHammerUI();
    }

    private void UpdateHammerUI()   //Cập nhật hiển thị Hammer trên UI
    {
        for (int i = 0; i < hammerBtnParent.transform.childCount; i++)
        {
            hammerBtnParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < hammerColorList.Count; i++)
        {
            hammerBtnParent.transform.GetChild(i).gameObject.SetActive(true);
            hammerBtnParent.transform.GetChild(i).GetComponent<Image>().color = BrickManager.Instance.colorData.colorMats[(int)hammerColorList[i]].color;
        }
    }
    #endregion
}