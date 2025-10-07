using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Alteruna;

public class BackgroundChange : AttributesSync
{
    [SerializeField] private LowerBlinds lowerBlindsTarget;

    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();
    [SerializeField] private GameObject objectToActivate;
    [SynchronizableField] private bool switchscene;

    public void Update()
    {
        if (switchscene)
        {
            
            TriggerChange();
        }
    }

    public void SceneSyncChange()
    {
        switchscene = true;
        Commit();
    }



    public void TriggerChange()
    {
        if (lowerBlindsTarget != null)
        {
            
            List<GameObject> activeObjects = new List<GameObject>();
            foreach (var obj in objectsToDeactivate)
            {
                if (obj != null && obj.activeSelf)
                {
                    activeObjects.Add(obj);
                }
            }

            
            lowerBlindsTarget.ChangeHeritage(activeObjects, objectToActivate);
        }
        else
        {
            Debug.Log("NO")
;        }
    }
}
