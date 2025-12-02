using UnityEngine;

public class enableUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;

private void Start()
    {
        canvas.enabled = false;
    }

    void Update()
    {
        // Check if a GameObject with the "Player" tag exists
        if (GameObject.FindWithTag("Player") == null)
        {
            return; // Don't allow toggling if no player exists
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }


}
