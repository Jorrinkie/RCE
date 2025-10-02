using UnityEngine;

public class Interactme : MonoBehaviour, IInteractable
{
    [SerializeField] private Animation animationComponent;
    [SerializeField] private string clipName = "ButtonPress";
    public bool issafe = true;
    private bool interacted = false;
    public GameObject OtherButton;

    private VoteManager voteManager;

    private void Start()
    {
        voteManager = FindObjectOfType<VoteManager>();
    }
    public void Interact()
    {
        if (interacted) return;

        Debug.Log($"{gameObject.name} was interacted with!");
        animationComponent.Play(clipName);

        if (issafe)
        {
            voteManager.addsafe();
        }
        else
        {
            voteManager.addsacrifice();
        }
        interacted = true;
        Destroy(gameObject, 1f);

            if (OtherButton != null)
    {
        
        Debug.Log("Other button exists!");
        OtherButton.SetActive(false);
    }
        
    }
    

    
}
