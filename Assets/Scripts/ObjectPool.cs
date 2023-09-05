    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Singleton
    public static ObjectPool Instance;
    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(this);
        }
        #endregion

        poolDictionary = new Dictionary<string, List<PooledObject>>();
        parentDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            GameObject parent = CreateNewPool(pool.tag);

            for (int i = 0; i < pool.size; i++)
            {
                PooledObject obj = CreateNewObject(pool.prefab.gameObject, parent.transform);
                poolDictionary[pool.tag].Add(obj);
            }
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

    /// <summary>
    /// Creating a GameObject that stores GameObjects with the same tag
    /// </summary>
    /// <param name="tag">The tag of the GameObject</param>
    /// <returns>The parent GameObject that can store GameObjects with the same tag name as the GameObject's name if there aren't any parent GameObject with the same name yet.</returns>
    private GameObject CreateNewPool(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("The tag named: " + tag + " is already defined, are you meaning to use another name?");
            return null;
        }

        GameObject parent = new GameObject(tag);
        parent.transform.SetParent(this.transform);

        List<PooledObject> list = new List<PooledObject>();

        poolDictionary.Add(tag, list);
        parentDictionary.Add(tag, parent);
        return parent;
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
            DestroyGameObject(pooledObject);
        }
        else if (saveToObjectPool) // If it doesn't have PooledObject Component but wants to be saved in ObjectPool
        {
            // Add the PooledObject component
            // If there aren't any parent GameObject with the same name as the GameObject's tag, make one
            // Save the object into the pool dictionary
            // Destroy the gameobject

            pooledObject = obj.AddComponent<PooledObject>();

            if (!poolDictionary.ContainsKey(obj.tag))
            {
                CreateNewPool(obj.tag);
            }

            poolDictionary[obj.tag].Add(pooledObject);
            pooledObject.DestroyObject();
        }
        else // If it doesn't want to be saved in ObjectPool
        {
            Destroy(obj);
        }

        return obj;
    }

    private GameObject DestroyGameObject(PooledObject pooledObject)
    {
        if (!poolDictionary.ContainsKey(pooledObject.tag))
        {
            CreateNewPool(pooledObject.tag);
        }

        if (pooledObject.transform.parent != parentDictionary[pooledObject.tag].transform)
        {
            pooledObject.transform.SetParent(parentDictionary[pooledObject.tag].transform);
        }

        pooledObject.DestroyObject();
        
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
                    DestroyGameObject(obj);
                }
            }
        }
    }

    public void DestroyAllGameObjectInPool()
    {
        DestroyAllGameObjectInPool(false);
    }
}
