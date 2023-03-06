using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData", order = 1)]
public class ColorData : ScriptableObject
{
    public ColorItem[] colorMats;

    public Material GetColorMat(ColorType colorType)
    {
        return colorMats[(int)colorType].material;
    }

    public Color GetColor(ColorType colorType)
    {
        return colorMats[(int)colorType].color;
    }

    public ColorType GetGreyColorType(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.RedBerry_1:
            case ColorType.Red_1:
            case ColorType.Orange_1:
            case ColorType.Yellow_1:
            case ColorType.Green_1:
            case ColorType.Cyan_1:
            case ColorType.Blue_1:
            case ColorType.SeaBlue_1:
            case ColorType.Purple_1:
            case ColorType.Magenta_1:
                colorType = ColorType.Grey_7;
                break;
            case ColorType.RedBerry_2:
            case ColorType.Red_2:
            case ColorType.Orange_2:
            case ColorType.Yellow_2:
            case ColorType.Green_2:
            case ColorType.Cyan_2:
            case ColorType.Blue_2:
            case ColorType.SeaBlue_2:
            case ColorType.Purple_2:
            case ColorType.Magenta_2:
                colorType = ColorType.Grey_3;
                break;
            case ColorType.RedBerry_3:
            case ColorType.Red_3:
            case ColorType.Orange_3:
            case ColorType.Yellow_3:
            case ColorType.Green_3:
            case ColorType.Cyan_3:
            case ColorType.Blue_3:
            case ColorType.SeaBlue_3:
            case ColorType.Purple_3:
            case ColorType.Magenta_3:
                colorType = ColorType.Grey_4;
                break;
            case ColorType.RedBerry_4:
            case ColorType.Red_4:
            case ColorType.Orange_4:
            case ColorType.Yellow_4:
            case ColorType.Green_4:
            case ColorType.Cyan_4:
            case ColorType.Blue_4:
            case ColorType.SeaBlue_4:
            case ColorType.Purple_4:
            case ColorType.Magenta_4:
                colorType = ColorType.Grey_5;
                break;
            case ColorType.RedBerry_5:
            case ColorType.Red_5:
            case ColorType.Orange_5:
            case ColorType.Yellow_5:
            case ColorType.Green_5:
            case ColorType.Cyan_5:
            case ColorType.Blue_5:
            case ColorType.SeaBlue_5:
            case ColorType.Purple_5:
            case ColorType.Magenta_5:
                colorType = ColorType.Grey_6;
                break;
            case ColorType.RedBerry_6:
            case ColorType.Red_6:
            case ColorType.Orange_6:
            case ColorType.Yellow_6:
            case ColorType.Green_6:
            case ColorType.Cyan_6:
            case ColorType.Blue_6:
            case ColorType.SeaBlue_6:
            case ColorType.Purple_6:
            case ColorType.Magenta_6:
                colorType = ColorType.Grey_8;
                break;
        }

        return colorType;
    }

    //public void SetupInit()
    //{
        //for (int i = 0; i < 72; i++)
        //{
        //    colorMats[i].material.color = colorMats[i].color;
        //}
    //}

}

[System.Serializable]
public class ColorItem
{
    public ColorType colorType;
    public Color color;
    public Material material;
}
