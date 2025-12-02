using UnityEngine;
using System.Collections.Generic;
using Alteruna;

public class BackgroundChange : AttributesSync
{
    [Header("References")]
    [SerializeField] private LowerBlinds lowerBlindsTarget;
    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();
    [SerializeField] private GameObject objectToActivate;

    public void SceneSyncChange()
    {
        Debug.Log("[BackgroundChange] Local player triggered scene change");

    
        TriggerChange();

   
        InvokeRemoteMethod(nameof(TriggerChange));
    }


    [SynchronizableMethod]
    private void TriggerChange()
    {
        Debug.Log("[BackgroundChange] TriggerChange() executed on: " + Multiplayer.Instance.Me);

        if (lowerBlindsTarget == null)
        {
            Debug.LogWarning("[BackgroundChange] No LowerBlinds target assigned!");
            return;
        }

        List<GameObject> activeObjects = new List<GameObject>();
        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null && obj.activeSelf)
                activeObjects.Add(obj);
        }

        lowerBlindsTarget.ChangeHeritage(activeObjects, objectToActivate);
    }
}
