using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Alteruna;

public class BackgroundChange : AttributesSync
{
    [Header("Background Settings")]
    [SerializeField] private LowerBlinds lowerBlindsTarget;
    [SerializeField] private GameObject myBackgroundRoot;
    [SerializeField] private string backgroundID = "tower";

    [Header("Tutorial Settings")]
    [SerializeField] private bool isTutorial = false;     // Only active in tutorial scene
    [SerializeField] private float tutorialDelay = 3f;    // Extra wait BEFORE background swaps

    // Normal online sync call
    public void SceneSyncChange()
    {
        BroadcastRemoteMethod(nameof(StartDelayedSwitch), backgroundID);
        StartDelayedSwitch(backgroundID);
    }

    [SynchronizableMethod]
    private void StartDelayedSwitch(string targetID)
    {
        StopAllCoroutines();
        StartCoroutine(DoSwitchAfterDelay(targetID));
    }

    private IEnumerator DoSwitchAfterDelay(string targetID)
    {
        Debug.Log($"[Background] Switching to {targetID}...");

        // If tutorial mode is enabled, use the custom delay
        if (isTutorial)
        {
            Debug.Log($"[Background] Tutorial mode ON — waiting {tutorialDelay} seconds...");
            yield return new WaitForSeconds(tutorialDelay);
        }
        else
        {
            // Normal small sync delay
            yield return new WaitForSeconds(2f);
        }

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

            bool shouldBeActive = string.Equals(
                bc.backgroundID,
                targetID,
                System.StringComparison.OrdinalIgnoreCase
            );

            if (bc.myBackgroundRoot.activeSelf && !shouldBeActive)
                deactivated.Add(bc.myBackgroundRoot);

            if (shouldBeActive)
                activated = bc.myBackgroundRoot;
        }

        
        StartCoroutine(DelayedBlindsSwap(deactivated, activated));
    }

    private IEnumerator DelayedBlindsSwap(List<GameObject> deactivated, GameObject activated)
    {
       
        if (isTutorial)
        {
            Debug.Log($"[Tutorial] Waiting {tutorialDelay} seconds before blinds swap...");
            yield return new WaitForSeconds(tutorialDelay);
        }

        // Blinds perform lower swap raise internally
        lowerBlindsTarget?.ChangeHeritage(deactivated, activated);
    }
}