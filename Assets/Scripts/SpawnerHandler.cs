using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerHandler : MonoBehaviour
{
    public bool IsEnabled = true;

    [Header("Ally Planes")]
    [SerializeField]
    private GameObject allyPlanePrefab;
    [SerializeField]
    private Transform allySpawnLocation;
    [SerializeField]
    private float allySpawnRadius = 1000f;
    [SerializeField]
    private int allyNumber = 5;

    [Header("Enemy Planes")]
    [SerializeField]
    private GameObject enemyPlanePrefab;
    [SerializeField]
    private Transform enemySpawnLocation;
    [SerializeField]
    private float enemySpawnRadius = 1000f;
    [SerializeField]
    private int enemyNumber = 5;

    void OnDrawGizmos()
    {
        if (IsEnabled)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(allySpawnLocation.position, allySpawnRadius);
            Gizmos.DrawWireSphere(enemySpawnLocation.position, enemySpawnRadius);
        }
    }

    void Update()
    {
        if (IsEnabled)
        {
            int curAllyNumber = GameObject.FindGameObjectsWithTag("Ally").Length;
            if (curAllyNumber < allyNumber)
            {
                SpawnPlane("Ally");
            }

            int curEnemyNumber = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (curEnemyNumber < enemyNumber)
            {
                SpawnPlane("Enemy");
            }
        }
    }

    void SpawnPlane(string tagType)
    {
        Vector3 spawnOffset = new Vector3();
        GameObject prefabObject = null;
        
        if (tagType.Equals("Ally"))
        {
            spawnOffset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized * allySpawnRadius;
            prefabObject = Instantiate(allyPlanePrefab, allySpawnLocation.position + spawnOffset, Quaternion.identity);
        }
        else
        {
            spawnOffset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized * enemySpawnRadius;
            prefabObject = Instantiate(enemyPlanePrefab, enemySpawnLocation.position + spawnOffset, Quaternion.identity);
        }

        Vector3 direction = transform.position - prefabObject.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        prefabObject.transform.rotation = rotation;
    }
}
