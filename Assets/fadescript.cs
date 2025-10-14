using UnityEngine;

public class fadescript : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 5f; // Time to fully fade out

    [SerializeField] private float durationTillStartOfFade = 5f;
    private Renderer rend;
    private Material initialMaterial;
    private float timer = 0f;
    private float initialTimer = 0f;
    void Start()
    {
        rend = GetComponent<Renderer>();
        initialMaterial = rend.material;
    }

    void Update()
    {
        initialTimer += Time.deltaTime;
        if (initialTimer <= durationTillStartOfFade)
            return;
        timer += Time.deltaTime;

        // Lerp alpha from original to 0
        float alpha = Mathf.Lerp(initialMaterial.color.a, 0f, timer / fadeDuration);

        Color newColor = initialMaterial.color;
        newColor.a = alpha;
        rend.material.color = newColor;

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
