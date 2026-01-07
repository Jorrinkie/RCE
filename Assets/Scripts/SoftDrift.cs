using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftDrift : MonoBehaviour
{
    [Header("Horizontal Drift")]
    public float minSpeedX = 0.1f;
    public float maxSpeedX = 0.3f;
    public float minAmpX = 0.1f;
    public float maxAmpX = 0.3f;

    [Header("Vertical Drift")]
    public float minSpeedY = 0.1f;
    public float maxSpeedY = 0.3f;
    public float minAmpY = 0.05f;
    public float maxAmpY = 0.2f;

    private float speedX, speedY;
    private float ampX, ampY;
    private float startX, startY;
    private float offsetX, offsetY;

    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;

        speedX = Random.Range(minSpeedX, maxSpeedX);
        ampX = Random.Range(minAmpX, maxAmpX);
        offsetX = Random.Range(0f, 100f);

        speedY = Random.Range(minSpeedY, maxSpeedY);
        ampY = Random.Range(minAmpY, maxAmpY);
        offsetY = Random.Range(0f, 100f);
    }

    void Update()
    {
        float x = startX + Mathf.Sin(Time.time * speedX + offsetX) * ampX;
        float y = startY + Mathf.Sin(Time.time * speedY + offsetY) * ampY;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}