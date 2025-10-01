using UnityEngine;
using Alteruna;
using TMPro;

public class Username : AttributesSync
{
    [SynchronizableField] public string userName = "Player";

    private Alteruna.Avatar _avatar;

    public TextMeshProUGUI nameText;
    public TMP_InputField inputField;

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

      
        if (nameText != null)
            nameText.text = userName;

    
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnNameChanged);
    }

    public void OnNameChanged(string newName)
    {
        if (_avatar == null || !_avatar.IsMe)
            return;

        if (string.IsNullOrEmpty(newName))
            return; 

        userName = newName;

        if (nameText != null)
            nameText.text = newName;

        Commit(); 
    }

    private void Update()
    {
        
        if (nameText != null && nameText.text != userName)
        {
            nameText.text = userName;
        }
    }
}
