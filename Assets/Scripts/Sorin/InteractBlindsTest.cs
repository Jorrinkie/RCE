using UnityEngine;

public class InteractBlindsTest : MonoBehaviour
{
    [SerializeField] private EnableOutline outlineChecker;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (outlineChecker.IsLookingAtHeritage)
            {
                var heritage = outlineChecker.CurrentTarget; 
                heritage?.GetComponent<BackgroundChange>()?.TriggerChange();
               
            }
        }
    }
}
