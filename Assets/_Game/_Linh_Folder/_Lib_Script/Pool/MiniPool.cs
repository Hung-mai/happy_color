using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniPool<T>
{
    private Queue<GameObject> queue = new Queue<GameObject>();
    private HashSet<GameObject> hash = new HashSet<GameObject>();
    private Dictionary<GameObject,T> dict = new Dictionary<GameObject,T>();

    GameObject prefab;
    Transform parent;

    public void OnInit(GameObject prefab, int amount, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        Preload(prefab, amount, parent);
    }

    private void Preload(GameObject prefab, int amount, Transform parent)
    {
        GameObject go = null;

        for (int i = 0; i < amount; i++)
        {
            go = GameObject.Instantiate(prefab, parent);
            go.SetActive(false);

            dict.Add(go, go.GetComponent<T>());
            queue.Enqueue(go);
        }
    }

    public T Spawn(Vector3 pos, Quaternion rot)
    {
        GameObject go = null;

        if (queue.Count > 0)
        {
            go = queue.Dequeue();
        }
        else
        {
            go = GameObject.Instantiate(prefab, parent);
            dict.Add(go, go.GetComponent<T>());
        }

        hash.Add(go);

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        return dict[go];
    }

    public void Despawn(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(false);
            queue.Enqueue(go);
            hash.Remove(go);
        }
    }
    
    public void DespawnAll()
    {
        HashSet<GameObject> hash = new HashSet<GameObject>(this.hash);

        foreach (var item in hash)
        {
            if (item.activeInHierarchy)
            {
                Despawn(item);
            }
        }
    }

    public void Release()
    {
        DespawnAll();

        while (queue.Count > 0)
        {
            GameObject go = queue.Dequeue();
            GameObject.DestroyImmediate(go);
        }
    }

}
