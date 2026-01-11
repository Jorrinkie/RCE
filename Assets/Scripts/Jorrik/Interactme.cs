using UnityEngine;

public class Interactme : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string boolParameter = "press"; 

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSound;


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
        
        if (audioSource != null && interactSound != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

       
        if (interacted) return;

        interacted = true;
        Debug.Log($"{gameObject.name} was interacted with!");

       
        if (animator != null)
            animator.SetBool(boolParameter, true);

       
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

       
        if (OtherButtons != null)
        {
            foreach (GameObject button in OtherButtons)
            {
                Interactme other = button.GetComponent<Interactme>();
                if (other != null)
                    other.SetInteracted(true);
            }
        }

        StartCoroutine(ResetAnimationBool());
    }

    private System.Collections.IEnumerator ResetAnimationBool()
    {
        yield return new WaitForSeconds(0.5f);

        if (animator != null)
            animator.SetBool(boolParameter, false);
    }

    public void SetInteracted(bool state)
    {
        interacted = state;
    }
}