using UnityEngine;
using System.Collections; // Needed for IEnumerator

public class LocalPauseScreen : MonoBehaviour
{
    private GameObject self;

    void Start()
    {
        self = gameObject;
        StartCoroutine(DisableAfterDelay(5f)); // wait 5 seconds
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        self.SetActive(false);
    }
}
