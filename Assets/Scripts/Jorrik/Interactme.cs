using UnityEngine;
using Alteruna;
public class Interactme : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string boolParameter = "press"; // Animator bool parameter name

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


        if (animator != null)
        {
            animator.SetBool(boolParameter, true);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no Animator assigned!");
        }

   
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

        // Optional: reset the animation bool after a short delay
        StartCoroutine(ResetAnimationBool());
    }

    private System.Collections.IEnumerator ResetAnimationBool()
    {
        // Wait for animation to finish (you can adjust duration)
        yield return new WaitForSeconds(0.5f);

        if (animator != null)
            animator.SetBool(boolParameter, false);
    }

    public void SetInteracted(bool state)
    {
        interacted = state;
    }
}
