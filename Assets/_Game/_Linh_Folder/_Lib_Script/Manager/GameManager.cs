using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AsyncOperation async;
    public GameState gameState = GameState.MainMenu;
    public GameMode gameMode = GameMode.Normal;
    /// <summary>
    /// khi update tien can phai update them cai gi day
    /// </summary>
    public GameState State => gameState;
    public float timeCounting = 0;
    public bool IsNoAds;
    public bool IsBuildMaketing;        //Build cho MKT thì ẩn tất cả các UI để làm Creative. (Tắt UI, Unlock tất cả level, tắt Ads)
    public bool IsEditor;               //Dùng để đánh dấu là đang tạo main level
    public List<BonusLevelData> bonusLevelInfo;
    public bool isJustOpened = true;    //Đánh dấu xem có phải người chơi vừa mở game lên chơi hay không. Nếu là vừa mở lên thì mới mở MainMenu. Không thì vào game luôn
    public GameMode[] listGameMode;     //List game mode của từng level
    public bool goHomeMenu = false;
    // Start is called before the first frame update
    public bool showSpecialOpenApp = false;
    public void Awake()
    {
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }

        BonusLevelData tempBonusLevelData = null;
        int index = 1;
        while (true)
        {
            tempBonusLevelData = Resources.Load<BonusLevelData>("BonusLevelInfo/BonusLevelData_" + index);
            if (tempBonusLevelData == null) break;
            bonusLevelInfo.Add(tempBonusLevelData);
            
            index++;
        }
        isJustOpened = true;
        timeCounting = 0;
    }

    

    private void Update()
    {
        timeCounting += Time.deltaTime;
    }

    public void ChangeState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public bool IsState(GameState gameState)
    {
        return this.gameState == gameState;
    }

    public Coroutine couroutine_countDown;
    public void CountDownSpecialOffer(int timeCount)
    {
        if(couroutine_countDown == null)
        {
            couroutine_countDown = StartCoroutine(ie_CountdownSpecialOffer(timeCount));
        }
    }

    [HideInInspector] public int timeCountSpecial;
    private IEnumerator ie_CountdownSpecialOffer(int timeCount)
    {
        for (timeCountSpecial = timeCount; timeCountSpecial > 0; timeCountSpecial--)
        {
            if(UI_Game.Instance != null)
            {
                string hours = ((timeCountSpecial / 60) / 60).ToString();
                string minutes = timeCountSpecial / 60 < 10 ? "0" + (timeCountSpecial / 60).ToString() : (timeCountSpecial / 60).ToString();
                string seconds = timeCountSpecial % 60 < 10 ? "0" + (timeCountSpecial % 60).ToString() : (timeCountSpecial % 60).ToString();
                string time = hours + ":" + minutes + ":" + seconds;
                if(UI_Game.Instance.IsOpenedUI(UIID.MainMenu))
                {
                    UI_Game.Instance.GetUI<CanvasMainMenu>(UIID.MainMenu).txt_time.text = time;
                }
                if(UI_Game.Instance.IsOpenedUI(UIID.GamePlay))
                {
                    UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).txt_timeSpecial.text = time;
                }
                if(UI_Game.Instance.IsOpenedUI(UIID.SpecialOffer))
                {
                    UI_Game.Instance.GetUI<CanvasSpecialOffer>(UIID.SpecialOffer).txt_time.text = time;
                }
            }

            yield return new WaitForSecondsRealtime(1);
        }

        
        // if(DataManager.Instance.gameData.indexSpecialOffer == 0)
        // {
        //     DataManager.Instance.gameData.levelGetFirework = DataManager.Instance.gameData.allLevelCounting;
        // }
        DataManager.Instance.gameData.specialShowing = false;
        if(DataManager.Instance.gameData.indexSpecialOffer < 2)
        {
            DataManager.Instance.gameData.specialItem[DataManager.Instance.gameData.indexSpecialOffer].isDoneOffer = true;
        }
        DataManager.Instance.gameData.indexSpecialOffer ++;

        // hết thời gian thì tự tắt 
        if(UI_Game.Instance.IsOpenedUI(UIID.MainMenu))
        {
            UI_Game.Instance.GetUI<CanvasMainMenu>(UIID.MainMenu).btn_specialOffer.SetActive(false);
        }
        if(UI_Game.Instance.IsOpenedUI(UIID.GamePlay))
        {
            UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btn_specialOffer.SetActive(false);
        }
        if(UI_Game.Instance.IsOpenedUI(UIID.SpecialOffer))
        {
            UI_Game.Instance.CloseUI(UIID.SpecialOffer);
        }

        StopCoroutine(couroutine_countDown);
        couroutine_countDown = null;
    }
}
