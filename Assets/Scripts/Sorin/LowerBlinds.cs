using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LowerBlinds : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float distance = 4f;
    [Header("Cube Blink Settings")]
    [SerializeField] private List<Renderer> cubeRenderers; // Assign the 6 cube Renderers in inspector
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] GameObject starterblack;
    private Coroutine blinkRoutine;
    private bool isMoving = false;
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
        // Trigger cube fade (blink)
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(FadeCubes());
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
    private IEnumerator FadeCubes()
    {
        if (cubeRenderers == null || cubeRenderers.Count == 0)
            yield break;
        // Start alpha at 0 (fully transparent)
        SetAlpha(0f);
        float alpha = 0f;
        // Fade in (transparent -> opaque)
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(alpha); // now alpha = 1 means fully opaque
            yield return null;
        }
        // Optional: hold fully opaque for a moment
        if (starterblack != null && starterblack.activeSelf == true)
        {
            starterblack.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(2f);

        // Fade out (opaque -> transparent)
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f); // Ensure fully transparent at the end
    }
    private void SetAlpha(float alpha)
    {
        foreach (var rend in cubeRenderers)
        {
            if (rend != null)
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = Mathf.Clamp01(alpha); // alpha = 0 -> transparent, 1 -> opaque
                        mat.color = c;
                        // Ensure the material is set to allow transparency
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        mat.SetInt("_ZWrite", 0);
                        mat.DisableKeyword("_ALPHATEST_ON");
                        mat.EnableKeyword("_ALPHABLEND_ON");
                        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat.renderQueue = 3000;
                    }
                }
            }
        }
    }
}