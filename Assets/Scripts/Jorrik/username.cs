using UnityEngine;
using Alteruna;
using TMPro;

public class Username : AttributesSync
{
    [SynchronizableField] public string userName = "Player";

    private Alteruna.Avatar _avatar;

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TMP_InputField inputField;

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        // Initialize UI
        UpdateNameText();

        // Only local player listens to input
        if (_avatar != null && _avatar.IsMe && inputField != null)
        {
            inputField.onValueChanged.AddListener(OnNameChanged);
        }
    }

    /// <summary>
    /// Called when the local player edits the username input field
    /// </summary>
    public void OnNameChanged(string newName)
    {
        if (_avatar == null || !_avatar.IsMe)
            return;

        if (string.IsNullOrEmpty(newName))
            return;

        // Only commit if the value actually changed
        if (userName != newName)
        {
            userName = newName;
            Commit(); // Synchronize with all clients
            UpdateNameText(); // Update local UI immediately
        }
    }

    private void Update()
    {
        // Update UI for remote clients
        if (_avatar != null && !_avatar.IsMe)
        {
            UpdateNameText();
        }
    }

    /// <summary>
    /// Updates the UI elements to match the current username
    /// </summary>
    private void UpdateNameText()
    {
        if (nameText != null)
            nameText.text = userName;

        // Update input field for local player
        if (_avatar != null && _avatar.IsMe && inputField != null)
            inputField.text = userName;
    }
}
