using Alteruna;
using UnityEngine;
using TMPro;

public class MyUsernameClass : CommunicationBridge
{
    public string myUsername = "Player";
    public TMP_InputField usernameInput;
    public TextMeshPro usernameText; // Assign a TextMeshPro above player

    void Awake()
    {
        if (PlayerPrefs.HasKey("Username"))
            myUsername = PlayerPrefs.GetString("Username");

        Multiplayer.SetUsername(myUsername);

        if (usernameInput != null)
            usernameInput.text = myUsername;

        if (usernameText != null)
            usernameText.text = myUsername;
    }

    public void OnUsernameInputChanged()
    {
        if (usernameInput != null)
            ChangeUsername(usernameInput.text);
    
    }

    public void ChangeUsername(string newUsername)
    {
        if (string.IsNullOrEmpty(newUsername)) return;

        myUsername = newUsername;

        PlayerPrefs.SetString("Username", myUsername);

        // Update Alteruna for the next session
        Multiplayer.SetUsername(myUsername);

        // Update local display immediately
        if (usernameText != null)
            usernameText.text = myUsername;

        Debug.Log("Username changed locally to: " + myUsername);
    }
}
