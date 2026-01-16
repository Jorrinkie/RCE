using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowerBlinds : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float distance = 4f;

    [Header("Cube Blink Settings")]
    [SerializeField] private List<Renderer> cubeRenderers;
    [SerializeField] private float fadeSpeed = 0.01f;
    [SerializeField] GameObject starterblack;

    private Coroutine blinkRoutine;
    private bool isMoving = false;

    // Used for tutorial mode ONLY (no fading)
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

        // Lower blinds 
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, loweredPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = loweredPos;

        // Change environment
        if (objectsToDeactivate != null)
            foreach (var obj in objectsToDeactivate)
                if (obj != null)
                    obj.SetActive(false);

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

    // Used for NON-Tutorial mode — fade only, no blinds
    public IEnumerator FadeSwap(List<GameObject> objectsToDeactivate, GameObject objectToActivate)
    {
        // Fade In 
        yield return StartCoroutine(FadeCubes(true));

        // Change background
        if (objectsToDeactivate != null)
            foreach (var obj in objectsToDeactivate)
                if (obj != null)
                    obj.SetActive(false);

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        // Fade Out 
        yield return StartCoroutine(FadeCubes(false));
    }

    private IEnumerator FadeCubes(bool fadeIn)
    {
        if (cubeRenderers == null || cubeRenderers.Count == 0)
            yield break;

        float alpha = fadeIn ? 0f : 1f;
        SetAlpha(alpha);

        while ((fadeIn && alpha < 1f) || (!fadeIn && alpha > 0f))
        {
            alpha += (fadeIn ? 1 : -1) * Time.deltaTime * fadeSpeed;
            alpha = Mathf.Clamp01(alpha);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(fadeIn ? 1f : 0f);
    }

    private void SetAlpha(float alpha)
    {
        foreach (var rend in cubeRenderers)
        {
            if (rend == null) continue;

            foreach (var mat in rend.materials)
            {
                if (!mat.HasProperty("_Color")) continue;

                Color c = mat.color;
                c.a = Mathf.Clamp01(alpha);
                mat.color = c;

                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}