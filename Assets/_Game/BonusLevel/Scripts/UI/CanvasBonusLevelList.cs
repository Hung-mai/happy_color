using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBonusLevelList : UICanvas
{
    public Transform levelList;
    public List<BonusLevelSelect> bonusLevelSelect;
    public Sprite unlockBG;
    private void Awake()
    {
        this.Setup();
    }
    public override void Setup()
    {
        base.Setup();
        for (int i = 0; i < bonusLevelSelect.Count; i++)
        {
            if (DataManager.Instance.gameData.currentPaintLevel-1 > i||GameManager.Instance.IsBuildMaketing) bonusLevelSelect[i].gameObject.SetActive(true);   //Chỉ hiển thị những bonus level nào đã được offer qua.
            else bonusLevelSelect[i].gameObject.SetActive(false);
            bonusLevelSelect[i].OnInit(i);
        }
    }
    public override void Open()
    {
        GameManager.Instance.ChangeState(GameState.Pause);
        base.Open();
    }

    public override void Close()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        GameManager.Instance.ChangeState(GameState.GamePlay);
        base.Close();
    }

    public void Btn_SelectBonusLevel(int index)
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        /*if (index > DataManager.Instance.gameData.currentLevel/5&&!GameManager.Instance.IsBuildMaketing) return;
        if (DataManager.Instance.gameData.paintLevelInformation[index - 1].isUnlock||GameManager.Instance.IsBuildMaketing)
        {
            DataManager.Instance.gameData.bonusDesireLevel = index;
            DataManager.Instance.SaveGame();
            LevelManager.Instance.LoadPaintLevel();
        }
        else
        {
            AppLovinController.instance.SetRewardPlacement(placement.Popup_Bonus_Level_Sellect);
            AppLovinController.instance.ShowRewardedAd(() =>
            {
                DataManager.Instance.gameData.paintLevelInformation[index - 1].isUnlock = true;
                Setup();
            });
        }*/
        DataManager.Instance.SaveGame();
    }
}
