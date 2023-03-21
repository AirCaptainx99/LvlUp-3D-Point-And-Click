using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [Header("Spawning Property")]
    public float spawnInterval;
    [SerializeField] private float nextTimeToSpawn;
    public int maxEnemyCount;
    [SerializeField] private int enemyCount;
    public float minX, minZ, maxX, maxZ;

    #region Singleton
    public static EnemySpawnController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    #endregion

    private void Start()
    {
        nextTimeToSpawn = Time.time;
    }
    private void Update()
    {
        if (Time.time - nextTimeToSpawn >= spawnInterval)
        {
            nextTimeToSpawn = Time.time + spawnInterval;

            if (enemyCount < maxEnemyCount)
            {
                GameObject enemyObject = ObjectPool.Instance.GetGameObjectFromPool("Enemy").gameObject;
                float xVal = Random.Range(minX, maxX);
                float zVal = Random.Range(minZ, maxZ);
                enemyObject.transform.position = new Vector3(xVal, 0, zVal);
            }
        }
    }

    public void IncrementEnemyCount()
    {
        enemyCount++;
    }

    public void DecrementEnemyCount()
    {
        enemyCount--;
    }
}
