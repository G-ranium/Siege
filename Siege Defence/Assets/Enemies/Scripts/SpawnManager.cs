using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public float spawnDelay = 2;
    public float spawnInterval = 1;
    public float radius = 15.0f;
    private Vector3 centerPoint;


    public enum SpawnDirection { North, East, South, West }
    public SpawnDirection spawnDirection = SpawnDirection.North;

    public int numberOfEnemiesToSpawn = 10;
    private int enemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        centerPoint = new Vector3(0, 1, 0);
    }

    // Spawn obstacles
    public void SpawnObjects()
    {
        if (enemiesSpawned >= numberOfEnemiesToSpawn) return;

        Vector3 spawnLocation = RandomPointInQuadrant();
        int index = Random.Range(0, objectPrefabs.Length);
        Instantiate(objectPrefabs[index], spawnLocation, objectPrefabs[index].transform.rotation);

        enemiesSpawned++;
    }

    public void StartSpawn()
    {
        if (enemiesSpawned < numberOfEnemiesToSpawn)
            InvokeRepeating("SpawnObjects", spawnDelay, spawnInterval);

        enemiesSpawned = 0;
    }

    private Vector3 RandomPointInQuadrant()
    {
        float angle = 0f;

        switch (spawnDirection)
        {
            case SpawnDirection.North:
                angle = Random.Range(315f, 360f) % 360f;
                if (Random.value < 0.5f) angle = Random.Range(0f, 45f);
                break;
            case SpawnDirection.East:
                angle = Random.Range(45f, 135f);
                break;
            case SpawnDirection.South:
                angle = Random.Range(135f, 225f);
                break;
            case SpawnDirection.West:
                angle = Random.Range(225f, 315f);
                break;
        }

        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians);
        float z = Mathf.Sin(radians);
        return centerPoint + new Vector3(x, 0, z) * radius;
    }
}