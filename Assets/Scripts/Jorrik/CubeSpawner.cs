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
        if (_spawner == null)
            return;
        int indexToSpawn = Random.Range(0, 10);
        // Spawn position slightly in front of camera
        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        // Random rotation for dynamic look
        Quaternion spawnRot = Random.rotation;

        // Spawn the cube
        GameObject cube = _spawner.Spawn(indexToSpawn, spawnPos, spawnRot, spawnScale);

        // Apply Rigidbody force if it exists
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Forward force
            Vector3 force = Camera.main.transform.forward * forwardForce;

            // Random side and up forces
            force += Camera.main.transform.right * Random.Range(-maxSideForce, maxSideForce);
            force += Camera.main.transform.up * Random.Range(0f, maxUpForce);

            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
