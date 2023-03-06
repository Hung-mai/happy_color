using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintEditor : MonoBehaviour
{
    public ColorButtonEditor[] colorButtonEditors;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < colorButtonEditors.Length; i++)
        {
            colorButtonEditors[i].SetColorType((ColorType)i);
        }       
    }

}
