using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int id;
    public bool isHaveItem = false;

    [Header("--------CurrentInSlot----------")]
    public Item itemInSlot;
    [Header("----------Ref------------")]
    public Transform targetSlot;
}
