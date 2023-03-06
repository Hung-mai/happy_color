using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR

public class CanvasEditor : Singleton<CanvasEditor>
{
    public BrickManagerEditor brickEditor;

    public Image choosingColor;
    public TextMeshProUGUI textColor;

    public TMP_InputField inputField;
    public TMP_InputField stepInputField;
    public TextMeshProUGUI levelText;

    public GameObject movePanel;

    public UserData userData;
    public ColorData colorData;
        
    private void Start()
    {
        levelText.SetText((brickEditor.levelDatas.Count + 1).ToString());
        UpdateImageColor();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetPaintType(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetPaintType(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetPaintType(2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetPaintType(3);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetPaintType(4);
        }   
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SetPaintType(5);
        }

    }

    public void ChangeColor(int colorType)
    {
        brickEditor.colorType = (ColorType)colorType;
        UpdateImageColor();
    }

    private void UpdateImageColor()
    {
        choosingColor.color = colorData.GetColor(brickEditor.colorType);
        textColor.SetText(brickEditor.colorType.ToString());
    }

    public void Save()
    {
        brickEditor.Save();
    }

    public void Load()
    {
        brickEditor.OnInit();

        if (int.Parse(inputField.text) <= brickEditor.levelDatas.Count)
        {
            levelText.SetText(inputField.text);
            brickEditor.Load(int.Parse(inputField.text) - 1);

        }

    }

    public void OnChangeStep()
    {
    }


    public void MoveButton()
    {
        movePanel.SetActive(!movePanel.activeInHierarchy);
    }

    public void FillCellButton()
    {
        brickEditor.FillCell();
    }

    public void SetPaintType(int paintType)
    {
        BrickManagerEditor.Instance.SetPaintStyle((BrickManagerEditor.PaintType)paintType);
    }

    public void PlayGameButton()
    {
        brickEditor.Save();
        userData.SetIntData(UserData.Key_Level, ref userData.PlayingLevel, int.Parse(levelText.text));
        SceneManager.LoadScene("MainScene");
    }
}

#endif
