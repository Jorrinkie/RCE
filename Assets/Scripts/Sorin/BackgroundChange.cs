using UnityEngine;
using System.Collections.Generic;
public class BackgroundChange : MonoBehaviour
{
    [SerializeField] private LowerBlinds lowerBlindsTarget;

    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();
    [SerializeField] private GameObject objectToActivate;

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
