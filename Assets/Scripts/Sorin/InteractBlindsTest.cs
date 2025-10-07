using UnityEngine;

public class InteractBlindsTest : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private EnableOutline outlineChecker;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        outlineChecker = player.GetComponent<EnableOutline>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (outlineChecker.IsLookingAtHeritage)
            {
                var heritage = outlineChecker.CurrentTarget;
                heritage?.GetComponent<BackgroundChange>()?.SceneSyncChange();
            }
        }
    }
}
