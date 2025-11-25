using Alteruna;
using UnityEngine;

public class CubePhysicsSync : AttributesSync
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This runs on ALL clients (network synchronized)
    [SynchronizableMethod]
    public void ApplyForce(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
