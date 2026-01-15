using UnityEngine;

public class IconManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationUI
    {
        public string name;
        public GameObject locationObject; // The Lighthouse/Windmill etc.
        public GameObject xIcon;          // Decision NOT made
        public GameObject checkIcon;      // Decision MADE
    }

    [Header("References")]
    [SerializeField] private VoteManager voteManager;
    [SerializeField] private LocationUI[] locations;

    private void Update()
    {
        if (voteManager == null) return;

        foreach (var loc in locations)
        {
            // We check the VoteManager to see if this specific location is resolved
            bool isResolved = voteManager.IsLocationResolved(loc.locationObject);

            if (loc.xIcon != null && loc.checkIcon != null)
            {
                // If resolved: Hide X, Show Checkmark
                // If not resolved: Show X, Hide Checkmark
                loc.xIcon.SetActive(!isResolved);
                loc.checkIcon.SetActive(isResolved);
            }
        }
    }
}

