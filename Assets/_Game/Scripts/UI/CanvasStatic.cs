using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasStatic : Singleton<CanvasStatic>
{
    public void SettingButton()
    {
        if (!GameManager.Instance.IsState(GameState.BlockAction) && !GameManager.Instance.IsState(GameState.Pause))
        {
            UI_Game.Instance.OpenUI(UIID.Setting);
        }
    }

    public void BtnBonusLevelList()
    {
        UI_Game.Instance.OpenUI(UIID.BonusLevelList);
    }
}
