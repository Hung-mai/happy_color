using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler> 
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string,Queue<GameObject>> dictionaryPool = new Dictionary<string,Queue<GameObject>>();

    public void Awake()
    {
        foreach (Pool p in pools)
        {
            Queue<GameObject> qObjPool = new Queue<GameObject>();
            for (int i = 0; i < p.size; i++)
            {
                GameObject obj = Instantiate(p.prefab);
                obj.SetActive(false) ;
                qObjPool.Enqueue(obj);//them vao queue
            }
            dictionaryPool.Add(p.tag,qObjPool);
        }
        
    }
    public void SpawnFromPool(string tag,Vector3 position,Vector3 eulerAngles)
    {
        if (!dictionaryPool.ContainsKey(tag))
        {
            Debug.LogError(tag + "not excist !");
            return;
        }
        GameObject obj = dictionaryPool[tag].Dequeue();
        if (obj != null)
        {
            obj.GetComponent<IObjectPooler>().OnSpawnObject();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.eulerAngles = eulerAngles;
        }
        
        dictionaryPool[tag].Enqueue(obj);
    }
}
