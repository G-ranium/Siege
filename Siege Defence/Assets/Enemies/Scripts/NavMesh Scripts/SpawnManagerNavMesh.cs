using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManagerNavMesh : MonoBehaviour
{
    public enum SpawnDirection { North, East, South, West }
    public SpawnDirection spawnDirection = SpawnDirection.North;
    public GameObject[] objectPrefabs;
    public IntData numberOfEnemiesToSpawn;
    public float spawnDelay = 2;
    public float spawnInterval = 1;
    public float radius = 15.0f;

    private Vector3 centerPoint;
    private GameObject spawner;
    private int enemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        int startingSpawn = 10;
        centerPoint = new Vector3(0, 1, 0);
        RandomizeSpawnDirection();
        numberOfEnemiesToSpawn.SetValue(startingSpawn);
    }

    // Spawn obstacles
    public void SpawnObjects()
    {
        if (enemiesSpawned >= numberOfEnemiesToSpawn.Value) return;

        Vector3 rawSpawnLocation = RandomPointInQuadrant();
        Vector3 spawnLocation = GetNearestNavMeshPoint(rawSpawnLocation);
        int index = Random.Range(0, objectPrefabs.Length);
        Instantiate(objectPrefabs[index], spawnLocation, objectPrefabs[index].transform.rotation);

        enemiesSpawned++;
    }

    private Vector3 GetNearestNavMeshPoint(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 5.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return position;
        }
    }

    public void StartSpawn()
    {
        if (enemiesSpawned < numberOfEnemiesToSpawn.Value)
            InvokeRepeating("SpawnObjects", spawnDelay, spawnInterval);

        enemiesSpawned = 0;
        UpdateNextWave();
    }

    public void UpdateNextWave()
    {
        double nextWave = numberOfEnemiesToSpawn.Value * 0.2;
        numberOfEnemiesToSpawn.UpdateValue((int)nextWave);
        RandomizeSpawnDirection();
    }

    void RandomizeSpawnDirection()
    {
        int directionIndex = Random.Range(0, 4);
        spawnDirection = (SpawnDirection)directionIndex;
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