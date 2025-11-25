using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Alteruna;

public class BackgroundChange : AttributesSync
{
    [SerializeField] private LowerBlinds lowerBlindsTarget;
    [SerializeField] private GameObject myBackgroundRoot;
    [SerializeField] private string backgroundID = "tower"; // ← "lighthouse", "tower", "church", etc.

    // Call this from your button / input
    public void SceneSyncChange()
    {
        // Tell EVERYONE (including self via RPC) to start the 1-second countdown
        BroadcastRemoteMethod(nameof(StartDelayedSwitch), backgroundID);
        StartDelayedSwitch(backgroundID); // also run locally immediately
    }

    [SynchronizableMethod]
    private void StartDelayedSwitch(string targetID)
    {
        StopAllCoroutines();                    // cancel any previous countdown
        StartCoroutine(DoSwitchAfterDelay(targetID));
    }

    private IEnumerator DoSwitchAfterDelay(string targetID)
    {
        // Optional: play sound, animation, fade, etc. here
        Debug.Log($"[Background] Switching to {targetID} in 1 second...");

        yield return new WaitForSeconds(0.1f);

        // NOW actually switch
        PerformBackgroundSwitch(targetID);
    }

    private void PerformBackgroundSwitch(string targetID)
    {
        var all = FindObjectsOfType<BackgroundChange>(true);
        var deactivated = new List<GameObject>();
        GameObject activated = null;

        foreach (var bc in all)
        {
            if (bc.myBackgroundRoot == null) continue;

            bool shouldBeActive = string.Equals(bc.backgroundID, targetID,
                System.StringComparison.OrdinalIgnoreCase);

            if (bc.myBackgroundRoot.activeSelf && !shouldBeActive)
                deactivated.Add(bc.myBackgroundRoot);

            bc.myBackgroundRoot.SetActive(shouldBeActive);

            if (shouldBeActive)
                activated = bc.myBackgroundRoot;
        }

        lowerBlindsTarget?.ChangeHeritage(deactivated, activated);
    }
}