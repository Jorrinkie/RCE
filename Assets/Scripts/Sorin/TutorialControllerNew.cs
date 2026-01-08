using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialControllerNew : MonoBehaviour
{
    [SerializeField] private AudioClip dutchClip;         
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject objectToActivate;

    private bool tutorialRunning;
    private Coroutine tutorialCoroutine;

    private void Awake()
    {
        objectToActivate.SetActive(false);
    }

    public void PlayTutorial()
    {
        if (tutorialRunning) return;

        tutorialRunning = true;
        tutorialCoroutine = StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        // Play animator trigger if assigned
        if (animator != null)
            animator.SetTrigger("Play");

        yield return new WaitForSeconds(0.2f);

        // Play the single Dutch clip
        if (dutchClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(dutchClip);
            yield return new WaitForSeconds(dutchClip.length);
        }

        // Activate the target object after audio finishes
        objectToActivate.SetActive(true);

        tutorialRunning = false;
    }

    public void SkipTutorial()
    {
        if (tutorialCoroutine != null)
            StopCoroutine(tutorialCoroutine);

        audioSource.Stop();
        tutorialRunning = false;
        objectToActivate.SetActive(true);
    }

    public void PlayButtonPressed()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("WorkingScene");
    }
}

