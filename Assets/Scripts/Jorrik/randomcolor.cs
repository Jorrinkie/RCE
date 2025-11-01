using UnityEngine;
using Alteruna;

public class PlayerColor : AttributesSync
{
    [SynchronizableField] public Color playerColor = Color.white;

    private Alteruna.Avatar _avatar;
    public Renderer avatarRenderer; // Assign your mesh/character Renderer in Inspector
    public Renderer avatarRenderer2; //for da arms;

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        // Only generate a random color for the local player
        if (_avatar != null && _avatar.IsMe)
        {
            playerColor = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f); // bright random color
            Commit(); // sync to everyone
        }

        ApplyColor();
    }

    private void Update()
    {
        ApplyColor(); // always ensure renderer shows synced color
    }

    private void ApplyColor()
    {
        if (avatarRenderer != null && avatarRenderer.material.color != playerColor)
        {
            avatarRenderer.material.color = playerColor;
            avatarRenderer2.material.color = playerColor;
        }
    }
}
