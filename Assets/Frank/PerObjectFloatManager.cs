using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PerObjectFloatManager : MonoBehaviour
{
    // This class lets us group an object with its own specific height setting
    [System.Serializable]
    public class FloatObjectData
    {
        public GameObject targetObject;
        public float heightToMove = 5.0f; // Unique to this object
        [HideInInspector] public Vector3 startPosition;
    }

    [Header("Timer Settings")]
    public int minutes = 30;
    public int seconds = 0;

    [Header("References")]
    public TextMeshProUGUI timerText;
    public List<FloatObjectData> objectSettings;

    private float _totalDuration;
    private float _elapsedTime;
    private bool _isMoving = false;

    void Start()
    {
        _totalDuration = (minutes * 60) + seconds;

        if (_totalDuration <= 0) return;

        // Store the starting world positions for everyone
        foreach (var data in objectSettings)
        {
            if (data.targetObject != null)
            {
                data.startPosition = data.targetObject.transform.position;
            }
        }

        _isMoving = true;
    }

    void Update()
    {
        if (!_isMoving) return;

        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _totalDuration);

        UpdateUI();
        ApplyCustomMovement(progress);

        if (progress >= 1.0f)
        {
            _isMoving = false;
            if (timerText != null) timerText.text = "Goal Reached";
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            float remaining = Mathf.Max(0, _totalDuration - _elapsedTime);
            int m = Mathf.FloorToInt(remaining / 60);
            int s = Mathf.FloorToInt(remaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", m, s);
        }
    }

    void ApplyCustomMovement(float progress)
    {
        foreach (var data in objectSettings)
        {
            if (data.targetObject != null)
            {
                // Each object calculates its own Y offset based on its specific height setting
                float myYOffset = progress * data.heightToMove;

                // Direct world position update (ignores scale)
                data.targetObject.transform.position = new Vector3(
                    data.startPosition.x,
                    data.startPosition.y + myYOffset,
                    data.startPosition.z
                );
            }
        }
    }
}