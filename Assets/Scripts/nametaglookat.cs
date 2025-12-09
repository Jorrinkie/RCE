using UnityEngine;

public class nametaglookat : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = transform.position.y;   // lock Y axis
        transform.LookAt(camPos);
    }
}
