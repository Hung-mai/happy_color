using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CanvasMainMenu : UICanvas
{
    public TextMeshProUGUI txt_time;
    public GameObject btn_specialOffer;
    public Animator animator;
    public RectTransform[] safeScreenBtn; //Những Button cần điều chỉnh khi dùng màn hình nốt ruồi
    private void Awake()
    {
        SetupSafeScreen();
    }
    public override void Setup()
    {
        base.Setup();
        GameManager.Instance.ChangeState(GameState.MainMenu);
    }

    public override void Open()
    {
        base.Open();
        CheckSpecialoffer(); 
    }

    private int levelMustCheck;
    private void CheckSpecialoffer()
    {
        int currentTime = (int) DateTime.Now.Subtract(Constant.TIME_MARKER).TotalSeconds;

        btn_specialOffer.SetActive(false);
        // TODO: nếu chưa mở thì mấy lần sau mở lên đều phải show ra
        switch (DataManager.Instance.gameData.indexSpecialOffer)
        {
            case 0: levelMustCheck = 6; break; // đến level 6 // nếu là phần special đầu tiên thì chỉ check hơn level 5
            case 1: levelMustCheck = 12; break; // đến level 12 // nếu là phần special thứ 2 thì chỉ check hơn level 12
            default: return;
        }
        
        if(DataManager.Instance.gameData.currentLevel >= levelMustCheck)
        {
            //if (/*DataManager.Instance.gameData.currentLevel % 3 != 0 && */DataManager.Instance.gameData.currentLevel > 5) return;
            if(currentTime > DataManager.Instance.gameData.timeEndSpecialOffer + Constant.TIME_SPECIAL_OFFER)  
            {
                if(DataManager.Instance.gameData.specialShowing == false) // lần đầu show
                {
                    DataManager.Instance.gameData.specialShowing = true;
                    GameManager.Instance.CountDownSpecialOffer(Constant.TIME_SPECIAL_OFFER);
                    DataManager.Instance.gameData.timeEndSpecialOffer = currentTime + Constant.TIME_SPECIAL_OFFER;
                    UI_Game.Instance.OpenUI(UIID.SpecialOffer);
                    btn_specialOffer.SetActive(true);
                }
                // else    // đã hết lúc mà off
                // {
                    
                //     DataManager.Instance.gameData.specialShowing = false;
                //     // if(DataManager.Instance.gameData.indexSpecialOffer == 0)
                //     // {
                //     //     DataManager.Instance.gameData.levelGetFirework = DataManager.Instance.gameData.allLevelCounting;
                //     // }
                //     if(DataManager.Instance.gameData.indexSpecialOffer < 2)
                //     {
                //         DataManager.Instance.gameData.specialItem[DataManager.Instance.gameData.indexSpecialOffer].isDoneOffer = true;
                //     }
                //     DataManager.Instance.gameData.indexSpecialOffer ++;
                //     btn_specialOffer.SetActive(false);
                // }
            }
            else if(currentTime > DataManager.Instance.gameData.timeEndSpecialOffer)
            {
                if(DataManager.Instance.gameData.specialShowing == true) // lần đầu show
                {
                    
                    DataManager.Instance.gameData.specialShowing = false;
                    if(DataManager.Instance.gameData.indexSpecialOffer < 2)
                    {
                        DataManager.Instance.gameData.specialItem[DataManager.Instance.gameData.indexSpecialOffer].isDoneOffer = true;
                    }
                    DataManager.Instance.gameData.indexSpecialOffer ++;
                    btn_specialOffer.SetActive(false);
                }
            }
            else if(currentTime < DataManager.Instance.gameData.timeEndSpecialOffer)  // current time < time end: tức là thời gian vẫn còn và sẽ tiếp tục chạy time
            {
                GameManager.Instance.CountDownSpecialOffer(DataManager.Instance.gameData.timeEndSpecialOffer - currentTime);
                // UI_Game.Instance.OpenUI(UIID.SpecialOffer);
                btn_specialOffer.SetActive(true);
            }
        }
        else
        {
            btn_specialOffer.SetActive(false);
        }
    }

    public void Btn_ShowSpecialOffer()
    {
        UI_Game.Instance.OpenUI(UIID.SpecialOffer);
    }

    void SetupSafeScreen()
    {
        if (Screen.height != Screen.safeArea.height)
        {
            for (int i = 0; i < safeScreenBtn.Length; i++)
            {
                Vector3 temp = safeScreenBtn[i].position;
                temp.y -= (Screen.height - Screen.safeArea.height);
                safeScreenBtn[i].position = temp;
            }
        }
    }

    public void OnPlayGame()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.BlockRaycast);
        animator.SetTrigger(Constant.ANIM_CLOSE);
        Invoke(nameof(Close), 0.65f);
    }

    public override void Close()
    {
        base.Close();
        LevelManager.Instance.OnPlay();
        GameManager.Instance.ChangeState(GameState.GamePlay);
        UI_Game.Instance.OpenUI(UIID.GamePlay);
        UI_Game.Instance.CloseUI(UIID.BlockRaycast);
    }

    public void SettingButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!GameManager.Instance.IsState(GameState.BlockAction) && !GameManager.Instance.IsState(GameState.Pause))
        {
            UI_Game.Instance.OpenUI(UIID.Setting);
        }
    }

    public void BtnBonusLevelList()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.BonusLevelList);
    }

    public void SellectGiftBtn()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.Property);
    }
}
