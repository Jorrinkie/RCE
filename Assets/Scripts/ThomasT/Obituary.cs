using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class ShowAbandonChurchVideo : MonoBehaviour
{
    [Header("UI References")]
    public VideoPlayer videoPlayer;       // Video to show
    public RawImage videoDisplay;         // The UI RawImage showing the video output
    public TextMeshProUGUI line1Text;     // "YOU HAVE DECIDED TO"
    public TextMeshProUGUI line2Text;     // "ABANDON"
    public TextMeshProUGUI line3Text;     // "THE CHURCH"

    [Header("Timing")]
    public float fadeInDuration = 2f;     // For first and last lines
    public float midDelay = 1f;           // Time between first fade and middle line
    public float visibleDuration = 5f;    // Time before fading starts
    public float fadeOutDuration = 3f;    // Fade-out length

    private bool isShowing = false;

    void Start()
    {
        // Hide everything initially
        SetActiveAll(false);
        if (videoPlayer != null)
            videoPlayer.targetTexture?.Release();
    }

    void Update()
    {
        if (!isShowing && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Showing Video!");
            StartCoroutine(ShowAndFadeSequence());
        }
    }

    IEnumerator ShowAndFadeSequence()
    {
        isShowing = true;

        // Activate video + text objects
        SetActiveAll(true);
        SetAlphaAll(0f);

        if (line1Text != null) line1Text.text = "THE CHURCH";
        if (line2Text != null) line2Text.text = "YOU HAVE DECIDED TO";
        if (line3Text != null) line3Text.text = "ABANDON";

        // Start video
        if (videoPlayer != null)
            videoPlayer.Play();

        // --- 1. Fade in first line ---
        if (line1Text != null)
            yield return StartCoroutine(FadeTextIn(line1Text, fadeInDuration));

        // --- 2. Wait before showing middle line ---
        yield return new WaitForSeconds(midDelay);

        // --- 3. Instantly show middle line ---
        if (line2Text != null)
        {
            SetAlpha(line2Text, 1f);
        }

        // --- 4. Fade in last line ---
        if (line3Text != null)
            yield return StartCoroutine(FadeTextIn(line3Text, fadeInDuration));

        // --- 5. Wait before fading out everything ---
        yield return new WaitForSeconds(visibleDuration);

        // --- 6. Fade out everything together ---
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            SetAlpha(videoDisplay, alpha);
            SetAlpha(line1Text, alpha);
            SetAlpha(line2Text, alpha);
            SetAlpha(line3Text, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(videoDisplay, 0f);
        SetAlpha(line1Text, 0f);
        SetAlpha(line2Text, 0f);
        SetAlpha(line3Text, 0f);
        SetActiveAll(false);

        if (videoPlayer != null)
            videoPlayer.Stop();

        isShowing = false;
    }

    IEnumerator FadeTextIn(TextMeshProUGUI textObj, float duration)
    {
        float elapsed = 0f;
        SetAlpha(textObj, 0f);
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            SetAlpha(textObj, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(textObj, 1f);
    }

    void SetActiveAll(bool active)
    {
        if (videoDisplay != null) videoDisplay.gameObject.SetActive(active);
        if (line1Text != null) line1Text.gameObject.SetActive(active);
        if (line2Text != null) line2Text.gameObject.SetActive(active);
        if (line3Text != null) line3Text.gameObject.SetActive(active);
    }

    void SetAlphaAll(float alpha)
    {
        if (videoDisplay != null) SetAlpha(videoDisplay, alpha);
        if (line1Text != null) SetAlpha(line1Text, alpha);
        if (line2Text != null) SetAlpha(line2Text, alpha);
        if (line3Text != null) SetAlpha(line3Text, alpha);
    }

    void SetAlpha(Graphic uiElement, float alpha)
    {
        if (uiElement == null) return;
        Color c = uiElement.color;
        c.a = alpha;
        uiElement.color = c;
    }

    void SetAlpha(TextMeshProUGUI tmp, float alpha)
    {
        if (tmp == null) return;
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }
}





