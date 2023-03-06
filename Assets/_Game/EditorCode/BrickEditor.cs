using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickEditor : MonoBehaviour
{
    public Vector2Int id;

    public MeshRenderer[] meshRenderers;

    public ColorData colorData;

    public BrickData brickData;

    public ColorType ColorType { 
        get 
        {
            if (brickData == null)
            {
                return ColorType.Editor;
            }

            return brickData.colorType;
        } 
    }

    public void OnInit()
    {
        SetColor(ColorType.Editor);
        brickData = null;
    }

    public void SetColor(ColorType colorType)
    {
        if (colorType == ColorType.Clear)
        {
            colorType = ColorType.Editor;
        }

        Material colorMat = colorData.GetColorMat(colorType);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = colorMat;
        }
    }

    public void SetParent(BrickData brickData)
    {
        if (this.brickData != null && this.brickData.listBricksID.Contains(id))
        {
            this.brickData.listBricksID.Remove(id);
        }

        this.brickData = brickData;

        if (brickData != null)
        {
            if (!brickData.listBricksID.Contains(id))
            {
                brickData.listBricksID.Add(id);
            }

            SetColor(brickData.colorType);
        }
        else
        {
            SetColor(ColorType.Editor);
        }
    }

    public void RemoveParent()
    {
        if (brickData != null)
        {
            brickData.listBricksID.Remove(id);
        }
    }
}
