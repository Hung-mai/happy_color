using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public enum UIID
{
    BlockRaycast, 
    GamePlay,
    MainMenu,
    Setting,
    MissionComplete,
    MissionFail,
    AirPlane,

    BonusGamePlay,
    BonusVictory,
    BonusLevelList,
    Rate,
    BonusLevelOffer,
    Tutorial,
    Loading,
    Property,
    LevelSortVictory,
    CanvasNewTheme,
    SpecialOffer,
}
public class UI_Game : Singleton<UI_Game>
{
    private Dictionary<UIID, UICanvas> UICanvas = new Dictionary<UIID, UICanvas>();

    public Transform CanvasParentTF;

    #region Canvas

    public bool IsOpenedUI(UIID ID)
    {
        return UICanvas.ContainsKey(ID) && UICanvas[ID] != null && UICanvas[ID].gameObject.activeInHierarchy;
    }

    private void Start()
    {
        int currentTime = (int) DateTime.Now.Subtract(Constant.TIME_MARKER).TotalSeconds;
        if(currentTime < DataManager.Instance.gameData.timeEndSpecialOffer)  // current time < time end: tức là thời gian vẫn còn và sẽ tiếp tục chạy time
        {
            if(GameManager.Instance.showSpecialOpenApp == false && GameManager.Instance.gameMode == GameMode.Normal)
            {
                GameManager.Instance.showSpecialOpenApp = true;
                UI_Game.Instance.OpenUI(UIID.SpecialOffer);
            }
        }
    }

    public UICanvas GetUI(UIID ID)
    {
        if (!UICanvas.ContainsKey(ID) || UICanvas[ID] == null)
        {
            if (ID != UIID.BonusVictory&&ID != UIID.MissionComplete)
            {
                UICanvas canvas = Instantiate(Resources.Load<UICanvas>("UI/" + ID.ToString()), CanvasParentTF);
                UICanvas[ID] = canvas;
            }
            else
            {
                UICanvas canvas = Instantiate(Resources.Load<UICanvas>("UI/" + ID.ToString()));
                UICanvas[ID] = canvas;
            }
        }

        return UICanvas[ID];
    } 
    
    public T GetUI<T>(UIID ID) where T : UICanvas
    {
        return GetUI(ID) as T;
    }

    public UICanvas OpenUI(UIID ID)
    {
        UICanvas canvas = GetUI(ID);

        canvas.Setup();
        canvas.Open();

        //TODO: REMOVE LATE
        //Cheat.Instance?.UpdateUI();
        if (ID == UIID.MissionComplete) 
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_end_ + (DataManager.Instance.gameData.currentLevel-1).ToString(), Parameter.level, (DataManager.Instance.gameData.currentLevel-1).ToString());
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_complete, Parameter.level, (DataManager.Instance.gameData.currentLevel-1).ToString());

            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_end_ + (((DataManager.Instance.gameData.currentLevel - 1) > 9) ? (DataManager.Instance.gameData.currentLevel - 1).ToString() : ("0" + (DataManager.Instance.gameData.currentLevel - 1).ToString())), (DataManager.Instance.gameData.currentLevel-1).ToString(), levelType.normal);
            FirebaseManager.Instance.LogLevelComplete(EventName.g_level_complete.ToString(), (DataManager.Instance.gameData.currentLevel-1).ToString(), Control.Instance.timeCounting);   
            Control.Instance.timeCounting = 0;
        }
        else if (ID == UIID.GamePlay|| ID == UIID.BonusGamePlay)
        {
            SoundController.Instance.FadeIn();
        }
        if (ID == UIID.MissionComplete|| ID == UIID.MissionFail|| ID == UIID.BonusVictory)
        {
            SoundController.Instance.FadeOut();
        }
        return canvas;
    }  
    
    public T OpenUI<T>(UIID ID) where T : UICanvas
    {
        return OpenUI(ID) as T;
    }

    public bool IsOpened(UIID ID)
    {
        return UICanvas.ContainsKey(ID) && UICanvas[ID] != null;
    }

    #endregion

    #region Back Button

    private Dictionary<UICanvas, UnityAction> BackActionEvents = new Dictionary<UICanvas, UnityAction>();
    private List<UICanvas> backCanvas = new List<UICanvas>();
    UICanvas BackTopUI {
        get
        {
            UICanvas canvas = null;
            if (backCanvas.Count > 0)
            {
                canvas = backCanvas[backCanvas.Count - 1];
            }
            return canvas;
        }
    }


    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape) && BackTopUI != null)
        {
            BackActionEvents[BackTopUI]?.Invoke();
        }
    }

    public void PushBackAction(UICanvas canvas, UnityAction action)
    {
        if (!BackActionEvents.ContainsKey(canvas))
        {
            BackActionEvents.Add(canvas, action);
        }
    }

    public void AddBackUI(UICanvas canvas)
    {
        if (!backCanvas.Contains(canvas))
        {
            backCanvas.Add(canvas);
        }
    }

    public void RemoveBackUI(UICanvas canvas)
    {
        backCanvas.Remove(canvas);
    }

    /// <summary>
    /// CLear backey when comeback index UI canvas
    /// </summary>
    public void ClearBackKey()
    {
        backCanvas.Clear();
    }

    #endregion

    public void CloseUI(UIID ID)
    {
        if (IsOpened(ID))
        {
            GetUI(ID).Close();
        }
    }
}
