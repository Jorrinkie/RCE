using UnityEngine;

public class Interactme : MonoBehaviour, IInteractable
{


    public int buttonNumber = 0;
    private bool interacted = false;

    [SerializeField] private GameObject[] OtherButtons;

    private VoteManager voteManager;

    private void Start()
    {
        voteManager = FindAnyObjectByType<VoteManager>();
    }

    public void Interact()
    {
        if (interacted) return;

        interacted = true;
        Debug.Log($"{gameObject.name} was interacted with!");




   
        if (buttonNumber == 0)
            voteManager.addsafe();
        else if (buttonNumber == 1)
            voteManager.addsacrifice();
        else if (buttonNumber == 2)
            voteManager.adddigitize();
        else if (buttonNumber == 3)
            voteManager.addRelocate();
        else if (buttonNumber == 4)
            voteManager.addRelocateBigVote();


        if (OtherButtons != null && OtherButtons.Length > 0)
        {
            foreach (GameObject button in OtherButtons)
            {
                if (button != null)
                {
                    Interactme other = button.GetComponent<Interactme>();
                    if (other != null)
                    {
                        other.SetInteracted(true);
                        Debug.Log($"Disabled interaction on: {button.name}");
                    }
                }
            }
        }


    }



    public void SetInteracted(bool state)
    {
        interacted = state;
    }
}
