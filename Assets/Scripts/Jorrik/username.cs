using UnityEngine;
using Alteruna;
using TMPro;

public class Username : AttributesSync
{
    [SynchronizableField] public string userName = "Player";

    private Alteruna.Avatar _avatar;

    [Header("UI References")]
    public TextMeshProUGUI nameText;      // Floating name above avatar
    public TMP_InputField inputField;      // Local input field

    private string lastUserName = "";

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        // Only local player listens to their input
        if (_avatar != null && _avatar.IsMe && inputField != null)
        {
            // Update final name when editing ends (Enter pressed or focus lost)
            inputField.onEndEdit.AddListener(OnNameEndEdit);

            // Keep local UI consistent
            inputField.text = userName;
        }

        UpdateUI();
        lastUserName = userName;
    }

    private void OnDestroy()
    {
        if (inputField != null)
            inputField.onEndEdit.RemoveListener(OnNameEndEdit);
    }

    /// <summary>
    /// Called when the local player finishes typing their name
    /// </summary>
    private void OnNameEndEdit(string newName)
    {
        if (_avatar == null || !_avatar.IsMe)
            return;

        if (string.IsNullOrEmpty(newName) || newName == userName)
            return;

        // Immediately update local UI
        nameText.text = newName;

        //  Send final name to host/owner to commit it
        BroadcastRemoteMethod("RPC_SetUsername", newName);
    }

    /// <summary>
    /// Host executes this method and commits the final username
    /// </summary>
    [SynchronizableMethod]
    private void RPC_SetUsername(string newName)
    {
        if (string.IsNullOrEmpty(newName) || newName == userName)
            return;

        userName = newName;
        Commit(); // Syncs to all clients
    }

    private void Update()
    {
        // Update UI if the synced value changes (from host commit)
        if (userName != lastUserName)
        {
            lastUserName = userName;
            UpdateUI();
        }
    }

    /// <summary>
    /// Updates name text and input field for local player
    /// </summary>
    private void UpdateUI()
    {
        if (nameText != null)
            nameText.text = userName;

        if (_avatar != null && _avatar.IsMe && inputField != null)
            inputField.text = userName;
    }
}
