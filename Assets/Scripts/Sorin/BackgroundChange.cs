using UnityEngine;
using System.Collections.Generic;
using Alteruna;

public class BackgroundChange : AttributesSync
{
    [Header("References")]
    [SerializeField] private LowerBlinds lowerBlindsTarget;
    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();
    [SerializeField] private GameObject objectToActivate;

    // Called locally by player input (E key) (E key scrapped because left click was found better - jorrik)
    public void SceneSyncChange()
    {
        Debug.Log("[BackgroundChange] Local player triggered scene change");

        //  Execute locally first
        TriggerChange();

        //  Call this method on all remote peers
        InvokeRemoteMethod(nameof(TriggerChange));
    }

    // Runs on all players' machines
    [SynchronizableMethod]
    private void TriggerChange()
    {
        Debug.Log("[BackgroundChange] TriggerChange() executed on: " + Multiplayer.Instance.Me);

        if (lowerBlindsTarget == null)
        {
            Debug.LogWarning("[BackgroundChange] No LowerBlinds target assigned!");
            return;
        }

        // Find currently active objects to deactivate
        List<GameObject> activeObjects = new List<GameObject>();
        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null && obj.activeSelf)
                activeObjects.Add(obj);
        }

        lowerBlindsTarget.ChangeHeritage(activeObjects, objectToActivate);
    }
}
