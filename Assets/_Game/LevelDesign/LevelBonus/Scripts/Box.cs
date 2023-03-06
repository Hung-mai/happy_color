using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Point[] points;

}
[System.Serializable]
public class Point
{
    public Transform transPoint;
    public Item item;
}
