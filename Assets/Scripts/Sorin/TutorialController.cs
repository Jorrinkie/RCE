using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private bool english;

    [SerializeField] private AudioClip englishClip1;
    [SerializeField] private AudioClip englishClip2;

    [SerializeField] private AudioClip dutchClip1;
    [SerializeField] private AudioClip dutchClip2;

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
        if (animator != null)
            animator.SetTrigger("Play");

        yield return new WaitForSeconds(0.2f);

        AudioClip clip1 = english ? englishClip1 : dutchClip1;
        AudioClip clip2 = english ? englishClip2 : dutchClip2;

        audioSource.PlayOneShot(clip1);
        yield return new WaitForSeconds(clip1.length);

        audioSource.PlayOneShot(clip2);
        yield return new WaitForSeconds(clip2.length);

        objectToActivate.SetActive(true);
       
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

    }
}