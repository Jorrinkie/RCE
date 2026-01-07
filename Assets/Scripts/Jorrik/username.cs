using UnityEngine;
using Alteruna;
using TMPro;

public class Username : AttributesSync
{
    [SynchronizableField] public string userName = "Player";
    private string _lastUserName; // Tracks changes locally

    private Alteruna.Avatar _avatar;

    [Header("UI References")]
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Start()
    {
        // Only the local player defines their name and pushes it to the network
        if (_avatar.IsMe)
        {
            string extractedName = ExtractNameFromGameObject(gameObject.name);
            if (!string.IsNullOrWhiteSpace(extractedName))
            {
                userName = extractedName;
                Commit(); // Pushes the name to all other players
            }
        }

        UpdateUI();
        _lastUserName = userName;
    }

    private void Update()
    {
        // Check if the userName has been changed by the network
        if (userName != _lastUserName)
        {
            _lastUserName = userName;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (nameText != null)
        {
            nameText.text = userName;
            // Debug.Log($"UI Updated for {gameObject.name} to: {userName}");
        }
    }

    private string ExtractNameFromGameObject(string objectName)
    {
        // Removes clones/IDs if Unity adds them, e.g., "Player(Bob)" -> "Bob"
        int start = objectName.IndexOf('(');
        int end = objectName.LastIndexOf(')'); // Use LastIndexOf for better safety
        if (start >= 0 && end > start)
        {
            return objectName.Substring(start + 1, end - start - 1);
        }
        return objectName;
    }
}