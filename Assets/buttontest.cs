using UnityEngine;
using UnityEngine.SceneManagement;

public class buttontest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonpress()
    {
        Debug.Log("buttonpressed");
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    public void ButtonPlay()
    {
        SceneManager.LoadScene("WorkingScene");
    }

    public void ButtonTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
