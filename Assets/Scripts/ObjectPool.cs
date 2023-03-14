    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Singleton
    public static ObjectPool Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(this);
        }
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public PooledObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, List<PooledObject>> poolDictionary;
    public Dictionary<string, GameObject> parentDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, List<PooledObject>>();
        parentDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            List<PooledObject> list = new List<PooledObject>();
            GameObject parent = new GameObject(pool.tag);

            for (int i = 0; i < pool.size; i++)
            {
                PooledObject obj = CreateNewObject(pool.prefab.gameObject, parent.transform);
                list.Add(obj);
            }

            poolDictionary.Add(pool.tag, list);
            parent.transform.SetParent(this.transform);

            parentDictionary.Add(pool.tag, parent);
        }
    }

    private PooledObject CreateNewObject(GameObject prefab, Transform parent)
    {
        GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newObj.SetActive(false);
        newObj.transform.SetParent(parent);

        return newObj.GetComponent<PooledObject>();
    }

    public PooledObject GetGameObjectFromPool(string tag)
    {
        return GetGameObjectFromPool(tag, -1f);
    }

    public PooledObject GetGameObjectFromPool(string tag, float time)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("No GameObject with tag " + tag);
            return null;
        }

        PooledObject obj = null;

        // find gameobject that's ready to use
        foreach (PooledObject pooledObject in poolDictionary[tag])
        {
            if (!pooledObject.isActiveAndEnabled)
            {
                obj = pooledObject;
                break;
            }
        }

        // if there are none that's ready to use, make one
        if (obj == null)
        {
            foreach (Pool pool in pools)
            {
                if (pool.tag == tag)
                {
                    obj = CreateNewObject(pool.prefab.gameObject, parentDictionary[tag].transform);
                    poolDictionary[tag].Add(obj.GetComponent<PooledObject>());
                    
                    break;
                }
            }
        }

        obj.SpawnObject(time);

        return obj;
    }

    public GameObject DestroyGameObject(GameObject obj)
    {
        return DestroyGameObject(obj, false);
    }

    public GameObject DestroyGameObject(GameObject obj, bool saveToObjectPool)
    {
        if (obj.TryGetComponent(out PooledObject pooledObject)) // If it has PooledObject Component
        {
            DestroyGameObject(pooledObject, saveToObjectPool);
        }
        else if (saveToObjectPool) // If it doesn't have PooledObject Component but wants to be saved in ObjectPool
        {
            
        }
        else // If it doesn't want to be saved in ObjectPool
        {
            Destroy(obj);
        }

        return obj;
    }

    public GameObject DestroyGameObject(PooledObject pooledObject, bool saveToObjectPool)
    {
        if (!poolDictionary.ContainsKey(pooledObject.tag))
        {
            Destroy(pooledObject.gameObject);
        }
        else
        {
            pooledObject.DestroyObject();
        }

        return pooledObject.gameObject;
    }

    public void DestroyAllGameObjectInPool(bool saveToObjectPool)
    {
        foreach (List<PooledObject> list in poolDictionary.Values)
        {
            foreach (PooledObject obj in list)
            {
                if (obj != null && obj.isActiveAndEnabled)
                {
                    DestroyGameObject(obj, saveToObjectPool);
                }
            }
        }
    }

    public void DestroyAllGameObjectInPool()
    {
        DestroyAllGameObjectInPool(false);
    }
}
