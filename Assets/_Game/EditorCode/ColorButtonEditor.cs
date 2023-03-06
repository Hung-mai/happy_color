using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonEditor : MonoBehaviour
{
    public ColorType colorType;
    public Image image;

    public ColorData colorData;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectColotButton);
    }

    public void SetColorType(ColorType colorType)
    {
        this.colorType = colorType;
        image.color = colorData.GetColor(colorType);
    }

    public void SelectColotButton()
    {
        //CanvasEditor.Instance.ChangeColor((int)colorType);
    }
}
