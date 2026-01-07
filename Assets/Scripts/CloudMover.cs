using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
    private float endX;
    private float speed;
    private CloudSpawner spawner;

    public void Initialize(float endX, float speed, CloudSpawner spawner)
    {
        this.endX = endX;
        this.speed = speed;
        this.spawner = spawner;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > endX)
        {
            if (spawner != null)
                spawner.CloudDestroyed();

            Destroy(gameObject);
        }
    }
}