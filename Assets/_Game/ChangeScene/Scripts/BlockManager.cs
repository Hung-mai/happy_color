using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
    public Transform tf;
    public BlockControl[,] blockList = new BlockControl[9,9];
    public Material blueMaterial;

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                tf.GetChild(index).position = new Vector3(j - 4, 0.1f, i - 4);
                blockList[i, j] = tf.GetChild(index).gameObject.GetComponent<BlockControl>();
                index++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TryPlayAnim());
        }
    }

    IEnumerator TryPlayAnim()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                blockList[j, i].PlayAnim();
                yield return Cache.GetWFS(Time.deltaTime / 1000f);
            }
            yield return Cache.GetWFS(Time.deltaTime / 1000f);
        }
    }
}
