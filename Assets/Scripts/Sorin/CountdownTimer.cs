using TMPro;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{

    [Header("Timer Settings")]
    [SerializeField] private float startTime = 20f;
    [SerializeField] private float warningTime = 10f; 
    private float currentTime;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Audio")]
    [SerializeField] private bool OnStart = true;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip warningSound;

    private bool timerRunning = false;
    private bool warningPlayed = false;

  
    public void StartCountdown()
    {
        currentTime = startTime;
        timerRunning = true;
        warningPlayed = false;


        if (OnStart && audioSource != null && startSound != null)
        {
            audioSource.clip = startSound;
            audioSource.volume = 1f;
            audioSource.Play();

            StartCoroutine(FadeOutAudio(5f)); 
        }
    }

    void Update()
    {
        if (!timerRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            if (timerText != null)
                timerText.text = $"{minutes:00}:{seconds:00}";

            // Warning sound
            if (!warningPlayed && currentTime <= warningTime)
            {
                warningPlayed = true;
                PlayWarningSound();
            }
        }
        else
        {
            currentTime = 0;
            timerRunning = false;

            Debug.Log("GAME OVER");
            // Add your game over logic here
        }
    }

    private void PlayWarningSound()
    {
        if (audioSource != null && warningSound != null)
        {
            audioSource.volume = 1f;
            audioSource.PlayOneShot(warningSound);
        }
    }

    private System.Collections.IEnumerator FadeOutAudio(float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audioSource.volume = 0f;
    }
}