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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!canvas.enabled)
            {
                canvas.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
            }
            else
            {
                canvas.enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
               
            }
        }
    }
}
