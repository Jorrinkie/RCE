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
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private float tutorialDelay = 3f;

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

        // Tutorial uses extra delay before blinds animation
        if (isTutorial)
        {
            yield return new WaitForSeconds(tutorialDelay);
        }
        else
        {
            // Normal fade mode: small delay
            yield return new WaitForSeconds(0.2f);
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
                bc.backgroundID, targetID, System.StringComparison.OrdinalIgnoreCase);

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
            // TUTORIAL MODE: blinds only (NO fade)
            lowerBlindsTarget?.ChangeHeritage(deactivated, activated);
        }
        else
        {
            // NORMAL MODE: fade-only swap
            yield return lowerBlindsTarget.StartCoroutine(
                lowerBlindsTarget.FadeSwap(deactivated, activated));
        }
    }
}