using UnityEngine;

public class checkforplayer : MonoBehaviour
{
    public GameObject uiToHide;

    void Update()
    {
        // Find an object with the tag "Player"
        GameObject player = GameObject.FindWithTag("Player");

        // If the player exists, deactivate the UI
        if (player != null)
        {
            uiToHide.SetActive(false);
        }
    }
}
