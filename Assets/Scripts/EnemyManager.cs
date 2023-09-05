using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Spawning Property")]
    public float spawnInterval;
    [SerializeField] private float nextTimeToSpawn;
    public int maxEnemyCount;
    [SerializeField] private int enemyCount;
    public float minX, minZ, maxX, maxZ;

    [Header("Enemy Pool Property")]
    public GameObject enemyPool;

    #region Singleton
    public static EnemyManager Instance;
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

        foreach (Transform pool in ObjectPool.Instance.transform)
        {
            if (pool.name == "Enemy")
            {
                enemyPool = pool.gameObject;
                break;
            }
        }
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

    public Character FindNearestEnemy(Vector3 playerPosition, float targetRange)
    {
        Transform nearestEnemy = null;
        float nearestDistance = float.PositiveInfinity;
        float distance;
        foreach (Transform enemy in enemyPool.transform)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                distance = Vector3.Distance(playerPosition, enemy.position);
                if (distance <= targetRange)
                {
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }
        }

        if (nearestEnemy == null)
        {
            return null;
        }
        else
        {
            nearestEnemy.TryGetComponent(out Character enemyTarget);
            return enemyTarget;
        }
    }
}
