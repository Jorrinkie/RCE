using TMPro;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float startTime = 30f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            currentTime = 0;

            // add logic later
        }
    }
}
