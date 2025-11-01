using UnityEngine;

public class rotate : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rotate around the X axis at a constant speed
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
