using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cheat : MonoBehaviour
{
    /*public TMP_InputField inputLevelCheat;
    public Toggle toggleHideUIGameplay;
    private void Awake()
    {
        toggleHideUIGameplay.isOn = GameManager.ins.isHideUIGameplay;
    }
    #region BUTTON
    public void BtnGoCheatLevel()
    {
        GameManager.ins.level = System.Convert.ToInt32(inputLevelCheat.text);
        Debug.Log(GameManager.ins.level);
        if (GameManager.ins.level > GameManager.ins.dataLevels.Length)
        {
            GameManager.ins.level = GameManager.ins.dataLevels.Length;
            DataManager.ins.gameSave.allLevelWin = GameManager.ins.level; //hieenr thi UI cho dung
        }
        else
        {
            DataManager.ins.gameSave.allLevelWin = GameManager.ins.level; //hieenr thi UI cho dung
        }
        DataManager.ins.gameSave.level = GameManager.ins.level;
        SceneController.ins.ChangeScene(Scene.Main);
    }
    public void OnChangeBoolToggle()
    {
        
        Debug.Log(toggleHideUIGameplay.isOn);
        if (toggleHideUIGameplay.isOn)
        {
            if (SceneController.ins.canvasgroup_form != null) SceneController.ins.canvasgroup_form.alpha = 0;
        }
        if (!toggleHideUIGameplay.isOn)
        {
            if (SceneController.ins.canvasgroup_form != null) SceneController.ins.canvasgroup_form.alpha = 1;
        }
        GameManager.ins.isHideUIGameplay = toggleHideUIGameplay.isOn;
    }
    #endregion*/
}
