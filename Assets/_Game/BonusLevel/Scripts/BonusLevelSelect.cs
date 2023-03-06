using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusLevelSelect : MonoBehaviour
{
    public Image bonusLevelImage;
    public GameObject lockImage;
    public Image lockImageSprite;
    public GameObject stars;
    public GameObject[] starList;
    public Image background;
    public CanvasBonusLevelList bonusLevelList;

    public void OnInit(int levelID)
    {
        bonusLevelImage.sprite = DataManager.Instance.bonusLevelImage[levelID];
        if (levelID >= (DataManager.Instance.gameData.currentLevel-1) / 5 && !DataManager.Instance.gameData.paintLevelInformation[levelID].isUnlock&&!GameManager.Instance.IsBuildMaketing)
        {
            bonusLevelImage.color = new Color32(0, 0, 0, 255);
        }
        else 
        {
            bonusLevelImage.color = new Color32(255, 255, 255, 255);
            background.sprite = bonusLevelList.unlockBG;
        }
        lockImage.SetActive(!DataManager.Instance.gameData.paintLevelInformation[levelID].isUnlock);
        if ((DataManager.Instance.gameData.currentLevel-1) / 5 <= levelID)
        {
            lockImageSprite.color = new Color32(100, 100, 100, 255);
        }
        else
        {
            lockImageSprite.color = new Color32(255, 255, 255, 255);
        }
        stars.SetActive(DataManager.Instance.gameData.paintLevelInformation[levelID].isUnlock);
        for (int i = 0; i < starList.Length; i++)
        {
            if (DataManager.Instance.gameData.paintLevelInformation[levelID].numberOfStar > i) starList[i].SetActive(true);
            else starList[i].SetActive(false);
        }
    }
}
