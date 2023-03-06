using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPoint : MonoBehaviour
{
    float time;

    private void OnEnable()
    {
        time = 1f;
    }

    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
