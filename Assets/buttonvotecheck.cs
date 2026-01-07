using UnityEngine;
using UnityEngine.UI;

public class BurgemeesterLockBtn : MonoBehaviour
{
    public VoteManager voteManager;
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        // De knop is alleen klikbaar als de VoteManager zegt dat alle stemmen binnen zijn
        if (voteManager != null)
        {
            _button.interactable = voteManager.allVotesIn;
        }
    }
}