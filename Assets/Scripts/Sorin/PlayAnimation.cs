using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private AnimationClip[] clips;

    [Header("Protest Sounds")]
    [SerializeField] private AudioClip[] protestSounds;
    [SerializeField] private float minProtestInterval = 3f;
    [SerializeField] private float maxProtestInterval = 8f;

    private Animator animator;
    private AudioSource audioSource;

    private static int currentProtesters = 0;
    private static int maxSimultaneousProtesters = 2;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogWarning("No Animator found on " + gameObject.name);
            return;
        }

        if (clips != null && clips.Length > 0)
        {
            PlayRandomAnimation();
        }

        if (protestSounds != null && protestSounds.Length > 0)
        {
            float initialDelay = Random.Range(0f, maxProtestInterval);
            StartCoroutine(PlayProtestSounds(initialDelay));
        }
    }

    private void PlayRandomAnimation()
    {
        AnimationClip randomClip = clips[Random.Range(0, clips.Length)];
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        overrideController["DefaultAnimation"] = randomClip;
        animator.runtimeAnimatorController = overrideController;
        animator.Play("DefaultAnimation");
    }

    private IEnumerator PlayProtestSounds(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            float waitTime = Random.Range(minProtestInterval, maxProtestInterval);
            yield return new WaitForSeconds(waitTime);

            if (audioSource != null && protestSounds.Length > 0)
            {
                // Only play if fewer than maxSimultaneousProtesters are currently playing
                if (currentProtesters < maxSimultaneousProtesters)
                {
                    currentProtesters++;

                    AudioClip clip = protestSounds[Random.Range(0, protestSounds.Length)];
                    audioSource.PlayOneShot(clip);

                    // Wait for clip length before freeing up the slot
                    yield return new WaitForSeconds(clip.length);

                    currentProtesters--;
                }
            }
        }
    }
}