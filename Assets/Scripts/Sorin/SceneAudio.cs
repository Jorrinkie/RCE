using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogWarning("SceneAudio: No AudioSource found on this object.");
    }

    private void OnEnable()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}

