using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasNewTheme : UICanvas
{
    public Sprite[] themeImageList;
    public Image themeImage; 
    public override void Open()
    {
        base.Open();
        themeImage.sprite = themeImageList[(DataManager.Instance.gameData.themeID+1)%3];
    }
    public void BtnClaim()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.themeID++;
        if (DataManager.Instance.gameData.themeID < 3) DataManager.Instance.gameData.currentThemeID = DataManager.Instance.gameData.themeID;
        else DataManager.Instance.gameData.currentThemeID = DataManager.Instance.gameData.targetThemeID;

        //Tìm target Theme
        do
        {
            DataManager.Instance.gameData.targetThemeID = Random.Range(0, 3);
        } while (DataManager.Instance.gameData.currentThemeID == DataManager.Instance.gameData.targetThemeID);
        DataManager.Instance.SaveGame();
        UI_Game.Instance.GetUI<CanvasBonusVictory>(UIID.BonusVictory).LoadNextLevel();
        base.Close();
    }
}
