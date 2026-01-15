using Alteruna;
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
    private Multiplayer multiplayer;

    private void Start()
    {
        voteManager = FindAnyObjectByType<VoteManager>();
        multiplayer = FindAnyObjectByType<Multiplayer>();
    }

    public void Interact()
    {
        if (audioSource && interactSound)
            audioSource.PlayOneShot(interactSound);

        int playerId = multiplayer.Me.Index;
        voteManager.RegisterVote(playerId, buttonNumber);

        interacted = true;

        if (animator)
            animator.SetBool(boolParameter, true);

        if (OtherButtons != null)
        {
            foreach (GameObject button in OtherButtons)
            {
                Interactme other = button.GetComponent<Interactme>();
                if (other != null)
                    other.SetInteracted(false);
            }
        }

        StartCoroutine(ResetAnimationBool());
    }

    private System.Collections.IEnumerator ResetAnimationBool()
    {
        yield return new WaitForSeconds(0.5f);

        if (animator)
            animator.SetBool(boolParameter, false);
    }

    public void SetInteracted(bool state)
    {
        interacted = state;
    }
}