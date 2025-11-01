using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LowerBlinds : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float distance = 4f;

    [Header("Blink Panel Settings")]
    [SerializeField] private Image blinkPanel;
    [SerializeField] private float fadeSpeed = 1f;

    private Coroutine blinkRoutine;
    private bool isMoving = false;
    private GameObject plyr;

    private void Start()
    {
        // Start coroutine to wait for the player and assign Blink panel
        StartCoroutine(WaitForPlayerAndAssignBlink());
    }

    private IEnumerator WaitForPlayerAndAssignBlink()
    {
        // Wait until a GameObject with tag "Player" exists
        while (plyr == null)
        {
            plyr = GameObject.FindWithTag("Player");
            yield return null; // wait one frame
        }

        // Wait an extra frame for children to initialize
        yield return null;

        // Search recursively for Image named "BlinkCanvas" in the player
        Image[] images = plyr.GetComponentsInChildren<Image>(true); // true = include inactive
        blinkPanel = null;

        foreach (Image img in images)
        {
            if (img.name == "Blink")
            {
                blinkPanel = img;
                break;
            }
        }

        if (blinkPanel != null)
        {
            Debug.Log("Blink panel assigned successfully!");
        }
        else
        {
            Debug.LogWarning("BlinkCanvas not found under Player prefab!");
        }
    }

    public void ChangeHeritage(List<GameObject> toDeactivate, GameObject toActivate)
    {
        if (!isMoving)
            StartCoroutine(LowerSwapLiftRoutine(toDeactivate, toActivate));
    }

    private IEnumerator LowerSwapLiftRoutine(List<GameObject> objectsToDeactivate, GameObject objectToActivate)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 loweredPos = startPos - new Vector3(0, distance, 0);
        float elapsed = 0f;

        // Trigger blink
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkEffect());

        // Lower blinds
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, loweredPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = loweredPos;



        // Swap environment
        if (objectsToDeactivate != null)
        {
            foreach (var obj in objectsToDeactivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        // Raise blinds
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(loweredPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        isMoving = false;
    }

    private IEnumerator BlinkEffect()
    {
        if (blinkPanel == null) yield break;

        float alpha = 0f;

        // Fade in
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }

        // Optional hold full black
        yield return new WaitForSeconds(1.5f);

        // Fade out
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }

        // Ensure final alpha = 0
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (blinkPanel != null)
        {
            Color c = blinkPanel.color;
            c.a = Mathf.Clamp01(alpha);
            blinkPanel.color = c;
        }
    }
}
