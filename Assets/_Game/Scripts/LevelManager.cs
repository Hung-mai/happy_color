using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : Singleton<LevelManager>
{
    public BrickManager brickManager;
    public List<LevelData> levelDatas = new List<LevelData>();
    public Transform brickParent;
    public LevelData levelDataBase;
    public GameObject[] themeList;
    // Start is called before the first frame update
    void Awake()
    {
        LevelData levelData = null;
        int i = 1;
        while (true)
        {
            levelData = Resources.Load<LevelData>("LevelData_new_" + i);

            if (levelData == null) break;
            levelDatas.Add(levelData);
            i++;
        }
        if (GameManager.Instance.IsEditor)
        {
            levelDataBase = Resources.Load<LevelData>("LevelData_Base");
            return;
        }
        GameManager.Instance.gameMode = GameMode.Normal;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        ChangeTheme();
        LoadLevel();
        if (GameManager.Instance.isJustOpened || GameManager.Instance.goHomeMenu)
        {
            UI_Game.Instance.OpenUI(UIID.MainMenu);
            GameManager.Instance.ChangeState(GameState.MainMenu);
            GameManager.Instance.isJustOpened = false;
            GameManager.Instance.goHomeMenu = false;
        }
        else
        {
            UI_Game.Instance.OpenUI(UIID.GamePlay);
            GameManager.Instance.ChangeState(GameState.GamePlay);
            HammerControl.Instance.MoveToPlayGame();
        }
    }

    public void OnReset()
    {
        brickManager.OnReset();
    }

    public void OnLoadLevel(int level)
    {
        if (GameManager.Instance.IsEditor)
        {
            brickManager.SetData(levelDataBase);
            return;
        }
        brickManager.SetData(levelDatas[level - 1]);
        SetLevelMapScale(level);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_start, Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
        HammerControl.Instance.SetHammerType((HammerType)DataManager.Instance.gameData.currentHammerTypeID);
        BrickManager.Instance.SetBrickType((BrickType)DataManager.Instance.gameData.currentBrickTypeID);
    }

    public void OnLoadNextLevel()
    {
        UI_Game.Instance.CloseUI(UIID.BlockRaycast);
        LoadLevel();
        UI_Game.Instance.OpenUI(UIID.GamePlay);
        HammerControl.Instance.MoveToPlayGame();
    }

    public void OnLoadNextLevelEffect()
    {
        UI_Game.Instance.CloseUI(UIID.BlockRaycast);
        LoadLevel();
    }

    public void ChangeLevelEffect()
    {
        StartCoroutine(StartChangeLevelEffect());
    }

    IEnumerator StartChangeLevelEffect()
    {
        HammerControl.Instance.MoveToHideHammer();
        yield return Cache.GetWFS(0.5f);
        BrickManager.Instance.ChangeLevelEffect();

        //OnLoadNextLevelEffect();
        //Timer.Schedule(this, 1f,()=>UI_Game.Instance.OpenUI(UIID.GamePlay));
        //Timer.Schedule(this, 1f,()=> HammerControl.Instance.MoveToPlayGame());

    }

    public void SetLevelMapScale(int level) //Cài đặt vị trí và Scale cho level
    {
        brickParent.localScale = levelDatas[(level - 1) % levelDatas.Count].mapScale;
        brickParent.position = levelDatas[(level - 1) % levelDatas.Count].mapPosition;
        HammerControl.Instance.TargetZPosition = -levelDatas[(level - 1) % levelDatas.Count].hammerPosition;
    }

    public void OnInit(int level)
    {
        brickManager.OnInit();
        HammerControl.Instance.OnInit(levelDatas[level - 1]);
    }

    public void OnPlay()
    {
        HammerControl.Instance.MoveToPlayGame();
    }


    public void LoadLevel()
    {
        if (GameManager.Instance.IsEditor)
        {
            OnReset();
            OnLoadLevel(0);
            return;
        }
        OnReset();
        OnLoadLevel((DataManager.Instance.gameData.currentLevel - 1) % levelDatas.Count + 1);
        OnInit((DataManager.Instance.gameData.currentLevel - 1) % levelDatas.Count + 1);
    }

    public void OnReplay()
    {
        OnReset();
        OnLoadLevel((DataManager.Instance.gameData.currentLevel - 1) % levelDatas.Count + 1);
        OnInit((DataManager.Instance.gameData.currentLevel - 1) % levelDatas.Count + 1);
        Timer.Schedule(this, 0.1f, () =>
        {
            HammerControl.Instance.MoveToPlayGame();
            UI_Game.Instance.OpenUI(UIID.GamePlay);
        });
    }

    public void LoadPaintLevel()
    {
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_start_bonus, Parameter.level, DataManager.Instance.gameData.bonusDesireLevel == -1 ? DataManager.Instance.gameData.currentPaintLevel : DataManager.Instance.gameData.bonusDesireLevel);
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.PaintLevelStr);
    }

    public void LoadSortLevelBonus()
    {
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_level_start, Parameter.level, (DataManager.Instance.gameData.currentLevel).ToString());
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_sortgame_level_start_ + ((DataManager.Instance.gameData.currentSortLevel > 9) ? DataManager.Instance.gameData.currentSortLevel.ToString() : ("0" + DataManager.Instance.gameData.currentSortLevel).ToString()), Parameter.level, (DataManager.Instance.gameData.allLevelCounting + 1).ToString());
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.SortLevelStr);
    }

    public void EnableToClickOnBrick() //Đổi lại layer để cho người chơi có thế click lên Brick
    {
        //if (DataManager.Instance.gameData.currentLevel != 1) return;
        HammerControl.Instance.raycastDistance = Mathf.Infinity;
        Control.Instance.raycastDistance = Mathf.Infinity;
    }

    public void UnEnableToClickOnBrick() //Đổi lại layer để cho người chơi ko thế click lên Brick
    {
        //if (DataManager.Instance.gameData.currentLevel != 1) return;
        HammerControl.Instance.raycastDistance = 0;
        Control.Instance.raycastDistance = 0;
    }

    public void ChangeTheme()
    {
        for (int i = 0; i < themeList.Length; i++)
        {
            if (i == DataManager.Instance.gameData.currentThemeID)
            {
                themeList[i].SetActive(true);
            }
            else themeList[i].SetActive(false);
        }
    }

    private int levelMustCheck;
    public void CheckSpecialoffer()
    {
        int currentTime = (int)DateTime.Now.Subtract(Constant.TIME_MARKER).TotalSeconds;

        // TODO: nếu chưa mở thì mấy lần sau mở lên đều phải show ra
        switch (DataManager.Instance.gameData.indexSpecialOffer)
        {
            case 0: levelMustCheck = 6; break; // đến level 6 // nếu là phần special đầu tiên thì chỉ check hơn level 5
            case 1: levelMustCheck = 12; break; // đến level 12 // nếu là phần special thứ 2 thì chỉ check hơn level 12
            default: return;
        }

        if (DataManager.Instance.gameData.currentLevel >= levelMustCheck)
        {
            if (currentTime > DataManager.Instance.gameData.timeEndSpecialOffer + Constant.TIME_SPECIAL_OFFER)
            {
                if (DataManager.Instance.gameData.specialShowing == false) // lần đầu show
                {
                    DataManager.Instance.gameData.specialShowing = true;
                    GameManager.Instance.CountDownSpecialOffer(Constant.TIME_SPECIAL_OFFER);
                    DataManager.Instance.gameData.timeEndSpecialOffer = currentTime + Constant.TIME_SPECIAL_OFFER;
                    UI_Game.Instance.OpenUI(UIID.SpecialOffer);
                    UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btn_specialOffer.SetActive(true);

                }
            }
            // else if (currentTime > DataManager.Instance.gameData.timeEndSpecialOffer)  // đã hết thời gian show ra
            // {
            //     if (DataManager.Instance.gameData.specialShowing == true)
            //     {

            //         DataManager.Instance.gameData.specialShowing = false;
            //         if (DataManager.Instance.gameData.indexSpecialOffer < 2)
            //         {
            //             DataManager.Instance.gameData.specialItem[DataManager.Instance.gameData.indexSpecialOffer].isDoneOffer = true;
            //         }
            //         DataManager.Instance.gameData.indexSpecialOffer++;
            //     }
            // }
            else if (currentTime < DataManager.Instance.gameData.timeEndSpecialOffer)  // current time < time end: tức là thời gian vẫn còn và sẽ tiếp tục chạy time
            {
                GameManager.Instance.CountDownSpecialOffer(DataManager.Instance.gameData.timeEndSpecialOffer - currentTime);
                // UI_Game.Instance.OpenUI(UIID.SpecialOffer);
            }
        }
    }
}
