using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttontest : MonoBehaviour
{
    public GameObject UI;
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

    public void UIVisible()
    {
        UI.SetActive(false);
    }
}
