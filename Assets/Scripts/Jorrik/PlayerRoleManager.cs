using UnityEngine;
using Alteruna;
using System.Collections;
using System.Collections.Generic;

public class PlayerRoleManager : AttributesSync
{
    [SynchronizableField] public string currentRole = "Pending...";

    private string _lastSyncRole;
    private Alteruna.Avatar _avatar;

    private void Awake()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Start()
    {
        if (_avatar.IsMe)
        {
            // Start a coroutine to wait for the network to catch up
            StartCoroutine(DelayedRoleAssignment());
        }
        else
        {
            // For remote players, just apply whatever role they already have
            ApplyRoleToLayer();
        }
    }

    private IEnumerator DelayedRoleAssignment()
    {
        // Wait for 0.2 seconds to ensure other players are spawned and synced
        yield return new WaitForSeconds(0.2f);

        DetermineAvailableRole();
        Commit();
        ApplyRoleToLayer();
    }

    private void Update()
    {
        // Sync check for everyone
        if (currentRole != _lastSyncRole)
        {
            _lastSyncRole = currentRole;
            ApplyRoleToLayer();
        }
    }

    private void DetermineAvailableRole()
    {
        string[] priorityRoles = { "Notaris", "Bouwvakker", "Burgemeester" };

        // Find all Role Managers
        PlayerRoleManager[] allManagers = FindObjectsOfType<PlayerRoleManager>();
        List<string> takenRoles = new List<string>();

        foreach (var manager in allManagers)
        {
            if (manager == this) continue;

            // Only count roles that are already finalized
            if (!string.IsNullOrEmpty(manager.currentRole) && manager.currentRole != "Pending...")
            {
                takenRoles.Add(manager.currentRole);
            }
        }

        string assigned = "Collega";

        foreach (string role in priorityRoles)
        {
            if (!takenRoles.Contains(role))
            {
                assigned = role;
                break;
            }
        }

        currentRole = assigned;
    }

    private void ApplyRoleToLayer()
    {
        // Don't set layer if we are still pending
        if (currentRole == "Pending...") return;

        int layerID = LayerMask.NameToLayer(currentRole);

        if (layerID != -1)
        {
            gameObject.layer = layerID;
            foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = layerID;
            }
        }
    }
}