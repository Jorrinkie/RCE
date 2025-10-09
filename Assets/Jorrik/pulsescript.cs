using UnityEngine;
using TMPro;

public class TextPulseAndRotate : MonoBehaviour
{
    [Header("Text Settings")]
    public TMP_Text text; // Assign your TextMeshPro text here

    [Header("Size Settings")]
    public float sizeAmplitude = 5f; // How much the text size will change
    public float sizeSpeed = 2f;     // How fast the text size pulses

    [Header("Rotation Settings")]
    public float rotationAmplitude = 10f; // Max rotation in degrees
    public float rotationSpeed = 2f;      // How fast the text rotates

    private float initialFontSize;
    private Quaternion initialRotation;

    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }

        initialFontSize = text.fontSize;
        initialRotation = text.rectTransform.rotation;
    }

    void Update()
    {
        // Pulse font size
        text.fontSize = initialFontSize + Mathf.Sin(Time.time * sizeSpeed) * sizeAmplitude;

        // Rotate text left and right
        float rotationZ = Mathf.Sin(Time.time * rotationSpeed) * rotationAmplitude;
        text.rectTransform.rotation = initialRotation * Quaternion.Euler(0, 0, rotationZ);
    }
}

