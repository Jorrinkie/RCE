using UnityEngine;

public class InteractBlindsTest : MonoBehaviour
{
    [SerializeField] private EnableOutline outlineChecker;

    private void Start()
    {
        if (outlineChecker == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                outlineChecker = player.GetComponent<EnableOutline>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (outlineChecker != null && outlineChecker.IsLookingAtHeritage)
            {
                var heritage = outlineChecker.CurrentTarget;
                heritage?.GetComponentInParent<BackgroundChange>()?.SceneSyncChange();
            }
        }
    }
}
