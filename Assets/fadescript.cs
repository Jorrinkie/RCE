using UnityEngine;

public class fadescript : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 2f; // Time to fully fade out

    private Renderer rend;
    private Color initialColor;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Create an instance of the material so we don't affect the original prefab
        rend.material = new Material(rend.material);

        // Make sure the material supports transparency
        // Use Standard Shader with Rendering Mode: Transparent
        initialColor = rend.material.color;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Lerp alpha from original to 0
        float alpha = Mathf.Lerp(initialColor.a, 0f, timer / fadeDuration);

        Color newColor = initialColor;
        newColor.a = alpha;
        rend.material.color = newColor;

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
