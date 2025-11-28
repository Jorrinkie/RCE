using UnityEngine;

public class QuitOnQ : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }

    void QuitGame()
    {
        // If running in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.Exit(0);
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a built game
        Application.Quit();
#endif
    }
}
     
   
