using UnityEngine;
using UnityEngine.Rendering;

public class IconYAxisSpinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField, Tooltip("Speed in degrees per second (positive = one direction, negative = reverse)")]
    private float rotationSpeed = 120f;

    [Header("Axis (only one active at a time)")]
    [SerializeField] private bool spinOnXAxis = false;
    [SerializeField] private bool spinOnYAxis = true;
    [SerializeField] private bool spinOnZAxis = false;

    [Header("Ghost Effects")]
    [SerializeField, Range(0.1f, 1f)]
    [Tooltip("Lower = more see-through / ghostly")]
    private float transparency = 0.65f;

    [SerializeField, Tooltip("Make the whole icon grayscale / desaturated")]
    private bool makeGrayscale = true;

    [SerializeField, Tooltip("Turn ghost effects on/off")]
    private bool applyGhostEffect = true;

    [Header("Ghost Position Offset (when ghost mode is active)")]
    [SerializeField, Tooltip("Offset on X axis (red)")]
    private float ghostXOffset = 0f;

    [SerializeField, Tooltip("Offset on Y axis (green) - usually the one you want")]
    private float ghostYOffset = 0.2f;

    [SerializeField, Tooltip("Offset on Z axis (blue)")]
    private float ghostZOffset = 0f;

    [SerializeField, Tooltip("How fast to move to the target position")]
    [Range(1f, 20f)]
    private float moveSpeed = 8f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private bool isURP;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Awake()
    {
        isURP = GraphicsSettings.currentRenderPipeline?.GetType().Name.Contains("Universal") ?? false;
        renderers = GetComponentsInChildren<Renderer>(true);
        propBlock = new MaterialPropertyBlock();

        // Capture the true spawn position ONLY once here
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }

    void OnEnable()
    {
        // On reactivation, immediately set to the correct target (prevents partial lerp states)
        // This "locks" it to either spawn point or spawn + offset, without cumulative shifts
        bool wantsGhost = applyGhostEffect;
        Vector3 offset = wantsGhost ? new Vector3(ghostXOffset, ghostYOffset, ghostZOffset) : Vector3.zero;
        targetPosition = originalPosition + offset;
        transform.localPosition = targetPosition;  // Snap to avoid mid-lerp glitches
    }

    void Update()
    {
        // Spinning
        Vector3 axis = spinOnXAxis ? Vector3.right :
                       spinOnZAxis ? Vector3.forward :
                       Vector3.up;

        transform.Rotate(axis * rotationSpeed * Time.deltaTime);

        // Ghost position
        bool wantsGhost = applyGhostEffect;
        Vector3 offset = wantsGhost ? new Vector3(ghostXOffset, ghostYOffset, ghostZOffset) : Vector3.zero;
        targetPosition = originalPosition + offset;

        // Smooth lerp (but since we snap in OnEnable, this handles in-game toggles smoothly)
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Visuals
        if (wantsGhost)
        {
            ApplyGhostEffect();
        }
        else
        {
            ResetMaterials();
        }
    }

    private void ApplyGhostEffect()
    {
        foreach (Renderer rend in renderers)
        {
            rend.GetPropertyBlock(propBlock);

            string colorProp = isURP ? "_BaseColor" : "_Color";
            Color col = propBlock.GetColor(colorProp);
            col.a = transparency;
            propBlock.SetColor(colorProp, col);

            if (makeGrayscale)
            {
                propBlock.SetColor(colorProp, new Color(0.85f, 0.85f, 0.85f, transparency));
            }

            rend.SetPropertyBlock(propBlock);
        }
    }

    private void ResetMaterials()
    {
        foreach (Renderer rend in renderers)
        {
            rend.SetPropertyBlock(null);
        }
    }
}