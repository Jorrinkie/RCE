using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class CubeSpawner : MonoBehaviour
{
    private Alteruna.Avatar _avatar;
    private Spawner _spawner;

    [Header("Spawn Force Settings")]
    public float forwardForce = 10f;
    public float maxSideForce = 1f;
    public float maxUpForce = 2f;

    [Header("Cube Scale (Only used if you actually want to override scale)")]
    public Vector3 spawnScale = new Vector3(100f, 100f, 100f);

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
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnCube();
        }
    }

    void SpawnCube()
    {
        if (_spawner == null) return;

        // Random index from 0 to 10 inclusive
        int randomIndex = Random.Range(0, 10);

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        Quaternion spawnRot;

        if (randomIndex == 5)
        {
            // Get player's Y rotation (not the camera)
            float cameraParentY = Camera.main.transform.parent.eulerAngles.y;

            // Apply -90 X rotation, player's Y rotation, 0 Z rotation
            spawnRot = Quaternion.Euler(-90f, cameraParentY, 0f);
        }
        else
        {
            spawnRot = Random.rotation;
        }
        GameObject cube = _spawner.Spawn(randomIndex, spawnPos, spawnRot, spawnScale);

        RigidbodySynchronizable rbSync = cube.GetComponent<RigidbodySynchronizable>();
        if (rbSync != null)
        {
            Vector3 force = Camera.main.transform.forward * forwardForce;
            force += Camera.main.transform.right * Random.Range(-maxSideForce, maxSideForce);
            force += Camera.main.transform.up * Random.Range(0f, maxUpForce);

            rbSync.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("RigidbodySynchronizable missing on spawned cube!");
        }
    }
}
