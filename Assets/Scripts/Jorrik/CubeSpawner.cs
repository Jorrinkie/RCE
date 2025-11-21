using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class CubeSpawner : MonoBehaviour
{
    private Alteruna.Avatar _avatar;
    private Spawner _spawner;
    [SerializeField] private int indexToSpawn = 0;

    [Header("Spawn Force Settings")]
    public float forwardForce = 10f;
    public float maxSideForce = 1f;
    public float maxUpForce = 2f;

    [Header("Cube Scale")]
    public Vector3 spawnScale = new Vector3(0.4f, 0.4f, 0.4f);

    private void Awake()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        GameObject manager = GameObject.FindGameObjectWithTag("NetworkManager");
        if (manager != null)
            _spawner = manager.GetComponent<Spawner>();
        else
            Debug.LogError("NetworkManager not found!");
    }

    private void Update()
    {
        if (!_avatar.IsMe)
            return; // Only local player can spawn

        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnCube();
        }
    }

    void SpawnCube()
    {
        if (_spawner == null) return;

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        Quaternion spawnRot = Random.rotation;

        GameObject cube = _spawner.Spawn(indexToSpawn, spawnPos, spawnRot, spawnScale);

        RigidbodySynchronizable rbSync = cube.GetComponent<RigidbodySynchronizable>();
        if (rbSync != null)
        {
            // Replicated force!
            Vector3 force = Camera.main.transform.forward * forwardForce;
            force += Camera.main.transform.right * Random.Range(-maxSideForce, maxSideForce);
            force += Camera.main.transform.up * Random.Range(0f, maxUpForce);
            rbSync.AddForce(force, ForceMode.Impulse);  // <-- This replicates to everyone
        }
        else
        {
            Debug.LogError("RigidbodySynchronizable missing on spawned cube!");
        }
    }
}

