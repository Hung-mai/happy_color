using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSetting : UICanvas
{
    public GameObject musicOn;
    public GameObject musicOff;
    public GameObject vibrationOn;
    public GameObject vibrationOff;
    public GameObject soundOn;
    public GameObject soundOff;

    public GameObject menuMusicOn;
    public GameObject menuMusicOff;
    public GameObject menuVibrationOn;
    public GameObject menuVibrationOff;
    public GameObject menuSoundOn;
    public GameObject menuSoundOff;

    public GameObject panel_Ingame;
    public GameObject panel_MainMenu;
    public GameObject panel_SellectLevel;
    public Text SellectLevel_Text;

    public override void Setup()
    {
        panel_Ingame.SetActive(GameManager.Instance.gameState == GameState.GamePlay);
        panel_MainMenu.SetActive(GameManager.Instance.gameState != GameState.GamePlay);
        panel_SellectLevel.SetActive(GameManager.Instance.IsBuildMaketing);
        base.Setup();
    }
    public override void Open()
    {
        base.Open();
        UpdateSettingState();
        if(GameManager.Instance.IsState(GameState.GamePlay)) GameManager.Instance.ChangeState(GameState.Pause);
    }

    public override void Close()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        base.Close();
        if (GameManager.Instance.IsState(GameState.Pause)) GameManager.Instance.ChangeState(GameState.GamePlay);
    }

    public void RetryButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        base.Close();
        AppLovinController.instance.SetInterPlacement(placement.Btn_replay_ingame);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
        if (GameManager.Instance.gameMode == GameMode.Normal)
        {
            LevelManager.Instance.LoadLevel();
            HammerControl.Instance.MoveToPlayGame();
            GameManager.Instance.gameState = GameState.GamePlay;
        }else if (GameManager.Instance.gameMode == GameMode.Paint)
        {
            BonusLevelManager.Instance.ResetLevel();
            if (GameManager.Instance.IsState(GameState.Pause)) GameManager.Instance.ChangeState(GameState.GamePlay);
        }
    }

    public void Btn_Music()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.isMusic = !DataManager.Instance.gameData.isMusic;
        SoundController.Instance.OnOffMusic(DataManager.Instance.gameData.isMusic);
        DataManager.Instance.SaveGame();
        UpdateSettingState();
    }

    public void Btn_Vibration()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.isVibrate = !DataManager.Instance.gameData.isVibrate;
        DataManager.Instance.SaveGame();
        UpdateSettingState();
    }

    public void Btn_Sound()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.isSound = !DataManager.Instance.gameData.isSound;
        DataManager.Instance.SaveGame();
        UpdateSettingState();
    }

    void UpdateSettingState()
    {
        musicOn.SetActive(DataManager.Instance.gameData.isMusic);
        musicOff.SetActive(!DataManager.Instance.gameData.isMusic);
        vibrationOn.SetActive(DataManager.Instance.gameData.isVibrate);
        vibrationOff.SetActive(!DataManager.Instance.gameData.isVibrate);
        soundOn.SetActive(DataManager.Instance.gameData.isSound);
        soundOff.SetActive(!DataManager.Instance.gameData.isSound);

        menuMusicOn.SetActive(DataManager.Instance.gameData.isMusic);
        menuMusicOff.SetActive(!DataManager.Instance.gameData.isMusic);
        menuVibrationOn.SetActive(DataManager.Instance.gameData.isVibrate);
        menuVibrationOff.SetActive(!DataManager.Instance.gameData.isVibrate);
        menuSoundOn.SetActive(DataManager.Instance.gameData.isSound);
        menuSoundOff.SetActive(!DataManager.Instance.gameData.isSound);
    }

    public void Btn_SellectLevel()
    {
        int desireLevel = int.Parse(SellectLevel_Text.text);
        if (desireLevel > 0&& desireLevel<=50)
        {
            DataManager.Instance.gameData.allLevelCounting = desireLevel-1;
            if (GameManager.Instance.listGameMode[DataManager.Instance.gameData.allLevelCounting] == GameMode.Normal)
            {
                int normalLevelCount = 0;
                for (int i = 0; i < GameManager.Instance.listGameMode.Length; i++)
                {
                    if (GameManager.Instance.listGameMode[i] == GameMode.Normal)
                    {
                        normalLevelCount++;
                    }
                    if (i == DataManager.Instance.gameData.allLevelCounting)
                    {
                        break;
                    }
                }
                DataManager.Instance.gameData.currentLevel = normalLevelCount;
                DataManager.Instance.SaveGame();
                LevelManager.Instance.LoadLevel();
                UI_Game.Instance.CloseUI(UIID.GamePlay);
                UI_Game.Instance.OpenUI(UIID.MainMenu);
            }
            else
            {
                int paintLevelCount = 0;
                int normalLevelCount = 0;
                for (int i = 0; i < GameManager.Instance.listGameMode.Length; i++)
                {
                    if (GameManager.Instance.listGameMode[i] == GameMode.Paint)
                    {
                        paintLevelCount++;
                    }
                    if (GameManager.Instance.listGameMode[i] == GameMode.Normal)
                    {
                        normalLevelCount++;
                    }
                    if (i == DataManager.Instance.gameData.allLevelCounting)
                    { 
                        break;
                    }
                }
                DataManager.Instance.gameData.currentPaintLevel = paintLevelCount;
                DataManager.Instance.gameData.currentLevel = normalLevelCount;
                DataManager.Instance.SaveGame();
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.PaintLevelStr);
            }
            Close();
        }
    }

    public void Btn_SellectSortLevel()
    {
        int desireLevel = int.Parse(SellectLevel_Text.text);
        if (desireLevel > 0)
        {
            DataManager.Instance.gameData.currentSortLevel = 1;
            DataManager.Instance.SaveGame();
            //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.SortLevelStr);
        }
    }
}
