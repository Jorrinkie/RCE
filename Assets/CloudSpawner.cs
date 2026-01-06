using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Prefabs")]
    public GameObject[] cloudPrefabs;

    [Header("Spawn Settings")]
    public float spawnCooldown = 3f;
    public float minY = -2f;
    public float maxY = 2f;
    public float spawnX = -10f;
    public float endX = 10f;

    [Header("Z Position Range")]
    public float minZ = 0f;
    public float maxZ = 0f;

    [Header("Cloud Size")]
    public float minScale = 0.8f;
    public float maxScale = 1.5f;

    [Header("Cloud Movement")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;

    [Header("Cloud Limits")]
    public int maxClouds = 6;

    private bool canSpawn = true;
    private int currentCloudCount = 0;

    void Update()
    {
        if (canSpawn && currentCloudCount < maxClouds)
        {
            StartCoroutine(SpawnCloud());
        }
    }

    private IEnumerator SpawnCloud()
    {
        canSpawn = false;

        // Pick a random cloud prefab
        GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

        // Random Y and Z positions
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);

        // Spawn cloud
        GameObject cloud = Instantiate(prefab, new Vector3(spawnX, y, z), Quaternion.identity);

        // Random scale
        float scale = Random.Range(minScale, maxScale);
        cloud.transform.localScale = new Vector3(scale, scale, scale);

        currentCloudCount++;

        // Random movement speed
        float speed = Random.Range(minSpeed, maxSpeed);

        // Add mover and pass spawner reference
        CloudMover mover = cloud.AddComponent<CloudMover>();
        mover.Initialize(endX, speed, this);

        // Wait cooldown
        yield return new WaitForSeconds(spawnCooldown);

        canSpawn = true;
    }

    public void CloudDestroyed()
    {
        currentCloudCount--;
    }
}