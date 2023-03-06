using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanvasRate : UICanvas
{
    public GameObject[] enableStars;
    public GameObject BtnRate;
    public GameObject BtnRateInActive;
    private int starRate=-1;
    public void Btn_StarSellect(int index)
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        starRate = index;
        for (int i = 0; i < enableStars.Length; i++)
        {
            if(i<index) enableStars[i].SetActive(true);
            else enableStars[i].SetActive(false);
        }
        BtnRate.SetActive(true);
        BtnRateInActive.SetActive(false);
    }

    public void Btn_Rate()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (starRate == -1) return;
        if (starRate < 5) 
        {
            UI_Game.Instance.OpenUI(UIID.MissionComplete);
            Close();
        }
        if (starRate == 5)
        {
            try
            {
                Application.OpenURL("http://play.google.com/store/apps/details?id=" + Application.identifier); //TODO: Chuyển tới trang đánh giá
                UI_Game.Instance.OpenUI(UIID.MissionComplete);
                Close();
            }
            catch (Exception x)
            {
                UI_Game.Instance.OpenUI(UIID.MissionComplete);
                Close();
            }
        }
    }

    public void Btn_Later()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.MissionComplete);
        Close();
    }

}
