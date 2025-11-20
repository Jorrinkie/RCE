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

    private void OnDestroy()
    {
        if (inputField != null)
            inputField.onValueChanged.RemoveListener(OnNameChanged);
    }

    private void OnNameChanged(string newName)
    {
        if (_avatar == null || !_avatar.IsMe)
            return;

        if (string.IsNullOrEmpty(newName))
            return;

        // Only commit if the value actually changed
        if (userName != newName)
        {
            userName = newName;
            Commit(); // Sync to all clients
        }
    }

    private void Update()
    {
        // Update UI text for everyone
        if (nameText != null)
            nameText.text = userName;

        // Keep input field consistent for local player
        if (_avatar != null && _avatar.IsMe && inputField != null)
            inputField.text = userName;
    }

    private void UpdateNameText()
    {
        if (nameText != null)
            nameText.text = userName;

        if (_avatar != null && _avatar.IsMe && inputField != null)
            inputField.text = userName;
    }
}
