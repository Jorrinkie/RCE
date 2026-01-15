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

    [Header("Ghost Position Offset")]
    [SerializeField, Tooltip("How much higher to float when ghost mode is active")]
    private float ghostYOffset = 0.2f;

    [SerializeField, Tooltip("How fast to move to the target position (units per second)")]
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

        originalPosition = transform.localPosition;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Spinning (always active)
        Vector3 axis = spinOnXAxis ? Vector3.right :
                       spinOnZAxis ? Vector3.forward :
                       Vector3.up;

        transform.Rotate(axis * rotationSpeed * Time.deltaTime);

        // Ghost mode position + visuals
        bool wantsGhost = applyGhostEffect;

        targetPosition = originalPosition + (wantsGhost ? new Vector3(0, ghostYOffset, 0) : Vector3.zero);

        // Smooth movement
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Visual effects
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