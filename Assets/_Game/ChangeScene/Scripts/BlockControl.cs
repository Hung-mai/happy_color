using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public Animator blockAnim;
    public Renderer blockRenderer;

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnim()
    {
        blockAnim.SetTrigger("updown");
    }

    public void ChangeColor()
    {
        blockRenderer.material = BlockManager.Instance.blueMaterial;
    }
}
