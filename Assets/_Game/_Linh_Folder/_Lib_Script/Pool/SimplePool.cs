///
/// Simple pooling for Unity.
///   Author: Martin "quill18" Glaude (quill18@quill18.com)
///   Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
///   UPDATES:
/// 	2015-04-16: Changed Pool to use a Stack generic. 


using UnityEngine;
using System.Collections.Generic;
using System;

public static class SimplePool
{
    const int DEFAULT_POOL_SIZE = 3;

    static Dictionary<int, Pool> pools = new Dictionary<int, Pool>();

    private static Transform root;

    public static Transform Root
    {
        get
        {
            if (root == null)
            {
                root = GameObject.FindObjectOfType<PoolControler>().transform;

                if (root == null)
                {
                    root = new GameObject("Pool").transform;
                }
            }

            return root;
        }
    }

    class Pool
    {
        Transform m_sRoot = null;

        bool m_collect;

        Queue<GameUnit> inactive;

        //collect obj active ingame
        HashSet<GameUnit> active;

        // The prefab that we are pooling
        GameUnit prefab;

        int m_Amount;

        public bool isCollect { get => m_collect; }

        // Constructor
        public Pool(GameUnit prefab, int initialQty, Transform parent, bool collect)
        {
            inactive = new Queue<GameUnit>(initialQty);
            m_sRoot = parent;
            this.prefab = prefab;
            m_collect = collect;
            if (m_collect) active = new HashSet<GameUnit>();
        }
        public int Count {
            get { return inactive.Count;}
        }
        // Spawn an object from our pool
        public GameUnit Spawn(Vector3 pos, Quaternion rot)
        {
            GameUnit obj = Spawn();

            obj.tf.SetPositionAndRotation( pos, rot);

            return obj;
        }

        public GameUnit Spawn()
        {
            GameUnit obj;
            if (inactive.Count == 0)
            {
                obj = (GameUnit)GameObject.Instantiate(prefab, m_sRoot);

                if (!pools.ContainsKey(obj.GetInstanceID()))
                    pools.Add(obj.GetInstanceID(), this);
            }
            else
            {
                // Grab the last object in the inactive array
                obj = inactive.Dequeue();

                if (obj == null)
                {
                    return Spawn();
                }
            }

            if (m_collect) active.Add(obj);

            obj.gameObject.SetActive(true);

            return obj;
        }

        // Return an object to the inactive pool.
        public void Despawn(GameUnit obj)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
                inactive.Enqueue(obj);
            }

            if (m_collect) active.Remove(obj);
        }

        public void Clamp(int amount) {
            while(inactive.Count> amount) {
                GameUnit go = inactive.Dequeue();
                GameObject.DestroyImmediate(go);
            }
        }
        public void Release() {
            while(inactive.Count>0) {
                GameUnit go = inactive.Dequeue();
                GameObject.DestroyImmediate(go);
            }
            inactive.Clear();
        }

        public void Collect()
        {
            HashSet<GameUnit> hash = new HashSet<GameUnit>(active);

            foreach (var item in hash)
            {
                Despawn(item);
            }
        }
    }

    // All of our pools
    static Dictionary<int, Pool> poolInstanceID = new Dictionary<int, Pool>();

    /// <summary>
    /// Init our dictionary.
    /// </summary>
    static void Init(GameUnit prefab = null, int qty = DEFAULT_POOL_SIZE, Transform parent = null, bool collect = false)
    {
        if (prefab != null && !IsHasPool(prefab.GetInstanceID()))
        {
            poolInstanceID.Add(prefab.GetInstanceID(), new Pool(prefab, qty, parent, collect));
        }
    }

    static public bool IsHasPool(int instanceID)
    {
        return poolInstanceID.ContainsKey(instanceID);
    }


    static public void Preload(GameUnit prefab, int qty = 1, Transform parent = null, bool collect = false)
    {
        if (prefab == null)
        {
            Debug.LogError(parent.name + " : IS EMPTY!!!");
            return;
        }

        Init(prefab, qty, parent, collect);

        // Make an array to grab the objects we're about to pre-spawn.
        GameUnit[] obs = new GameUnit[qty];
        for (int i = 0; i < qty; i++)
        {
            obs[i] = Spawn<GameUnit>(prefab, Vector3.zero, Quaternion.identity);        
        }

        // Now despawn them all.
        for (int i = 0; i < qty; i++)
        {
            Despawn(obs[i]);
        }
    }

    static public T Spawn<T>(GameUnit obj, Vector3 pos, Quaternion rot) where T : GameUnit
    {
        return Spawn(obj, pos, rot) as T;
    }   

    static public T Spawn<T>(GameUnit obj) where T : GameUnit
    {
        if (!poolInstanceID.ContainsKey(obj.GetInstanceID()))
        {
            Transform newRoot = new GameObject(obj.name).transform;
            newRoot.SetParent(Root);
            Preload(obj, 1, newRoot, true);
        }

        GameUnit unit = poolInstanceID[obj.GetInstanceID()].Spawn();
        return unit as T;
        //return poolInstanceID[obj.GetInstanceID()].Spawn();
    }

    static public GameUnit Spawn(GameUnit obj, Vector3 pos, Quaternion rot)
    {
        if (!poolInstanceID.ContainsKey(obj.GetInstanceID()))
        {
            Transform newRoot = new GameObject(obj.name).transform;
            newRoot.SetParent(Root);
            Preload(obj, 1, newRoot, true);
        }

        return poolInstanceID[obj.GetInstanceID()].Spawn(pos, rot);
    }

    static public void Despawn(GameUnit obj)
    {
        if (obj.gameObject.activeSelf)
        {
            if (pools.ContainsKey(obj.GetInstanceID()))
                pools[obj.GetInstanceID()].Despawn(obj);
            else
                GameObject.Destroy(obj.gameObject);    
        }
    }

    static public void Release(GameUnit obj)
    {
        if (pools.ContainsKey(obj.GetInstanceID()))
        {
            pools[obj.GetInstanceID()].Release();
            pools.Remove(obj.GetInstanceID());
        }
        else
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    static public void Collect(GameUnit obj)
    {
        if (poolInstanceID.ContainsKey(obj.GetInstanceID()))
            poolInstanceID[obj.GetInstanceID()].Collect();
    }

    static public void CollectAll()
    {
        foreach (var item in poolInstanceID)
        {
            if (item.Value.isCollect)
            {
                item.Value.Collect();
            }
        }
    }
}

[System.Serializable]
public class PoolAmount
{
    [Header("-- Pool Amount --")]
    public Transform root;
    public GameUnit prefab;
    public int amount;
    public bool collect;
}

public class GameUnit: MonoBehaviour
{
    private Transform trans;
    public Transform tf 
    { 
        get
        {
            if (trans == null)
            {
                trans = gameObject.transform;
            }
            return trans;
        }
    }

    public virtual void OnInit() { }

}