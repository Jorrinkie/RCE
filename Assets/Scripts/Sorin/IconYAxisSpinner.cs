using UnityEngine;
using UnityEngine.Rendering;

public class IconYAxisSpinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField, Tooltip("Speed in degrees per second")]
    private float rotationSpeed = 120f;

    [Header("Axis")]
    [SerializeField] private bool spinOnXAxis = false;
    [SerializeField] private bool spinOnYAxis = true;
    [SerializeField] private bool spinOnZAxis = false;

    [Header("Ghost Effects")]
    [SerializeField, Range(0.1f, 1f)]
    private float transparency = 0.65f;

    [SerializeField, Tooltip("Make the whole icon grayscale / desaturated")]
    private bool makeGrayscale = true;

    [SerializeField, Tooltip("Turn ghost effects on/off")]
    private bool applyGhostEffect = true;

    [Header("Ghost Position Offset (when ghost mode is active)")]
    [SerializeField, Tooltip("Offset on X axis")]
    private float ghostXOffset = 0f;

    [SerializeField, Tooltip("Offset on Y axis")]
    private float ghostYOffset = 0.2f;

    [SerializeField, Tooltip("Offset on Z axis")]
    private float ghostZOffset = 0f;

    [Range(1f, 20f)]
    private float moveSpeed = 8f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private bool isURP;
    private Vector3 originalPosition;
    private Vector3 targetPosition;


    [SerializeField] private bool turnOff = false;

    void Awake()
    {
        isURP = GraphicsSettings.currentRenderPipeline?.GetType().Name.Contains("Universal") ?? false;
        renderers = GetComponentsInChildren<Renderer>(true);
        propBlock = new MaterialPropertyBlock();

        originalPosition = transform.localPosition;
        targetPosition = originalPosition;

        if(turnOff)
        {
            StartCoroutine(DelayedDeactivate(2f));
        }
    }

    private System.Collections.IEnumerator DelayedDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {    
        bool wantsGhost = applyGhostEffect;
        Vector3 offset = wantsGhost ? new Vector3(ghostXOffset, ghostYOffset, ghostZOffset) : Vector3.zero;
        targetPosition = originalPosition + offset;
        transform.localPosition = targetPosition; 
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