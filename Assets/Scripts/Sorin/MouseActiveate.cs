using UnityEngine;

public class MouseActiveate : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;     
        Cursor.visible = true;
    }
}
