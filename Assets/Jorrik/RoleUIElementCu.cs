using UnityEngine;
using Alteruna;

public class RoleVisibilityReactive : MonoBehaviour
{
    [Header("Instellingen")]
    public string targetRoleName = "Notaris";
    public string playerTag = "Player"; // De tag die je aan je player prefab hebt gegeven
    public bool invertSelection = false;

    private GameObject _localPlayer;
    private float _timer = 0f;
    private float _maxWaitTime = 3f;
    private bool _playerDetectedInScene = false;

    void OnEnable()
    {
        _timer = 0f;
        _playerDetectedInScene = false;
        _localPlayer = null;
    }

    void Update()
    {
        // 1. Wacht tot er überhaupt een speler in de scene is
        if (!_playerDetectedInScene)
        {
            if (GameObject.FindGameObjectWithTag(playerTag) != null)
            {
                _playerDetectedInScene = true;
                Debug.Log("[UI] Speler gedetecteerd in scene. Timer start nu.");
            }
            return; // Stop hier zolang er geen speler is
        }

        // 2. Vanaf hier gaat de timer pas lopen
        if (_timer > _maxWaitTime) return;
        _timer += Time.deltaTime;

        // 3. Zoek specifiek naar de LOKALE speler
        if (_localPlayer == null)
        {
            FindLocalPlayer();
            return;
        }

        string playerLayerName = LayerMask.LayerToName(_localPlayer.layer);

        // 4. Check de rol
        if (playerLayerName == targetRoleName)
        {
            Apply(true);
            this.enabled = false;
        }
        else if (playerLayerName != "Collega" && playerLayerName != "Default" && !string.IsNullOrEmpty(playerLayerName))
        {
            Apply(false);
            this.enabled = false;
        }
        else if (_timer > 2.0f) // Iets langer wachten voor netwerk sync
        {
            Apply(playerLayerName == targetRoleName);
            this.enabled = false;
        }
    }

    private void FindLocalPlayer()
    {
        Alteruna.Avatar[] avatars = FindObjectsOfType<Alteruna.Avatar>();
        foreach (var av in avatars)
        {
            if (av.IsMe)
            {
                _localPlayer = av.gameObject;
                break;
            }
        }
    }

    private void Apply(bool shouldBeVisible)
    {
        bool finalState = invertSelection ? !shouldBeVisible : shouldBeVisible;
        gameObject.SetActive(finalState);

        Debug.Log($"[UI Final] Check klaar. Player Layer: {LayerMask.LayerToName(_localPlayer.layer)}. Zichtbaar: {finalState}");
    }
}