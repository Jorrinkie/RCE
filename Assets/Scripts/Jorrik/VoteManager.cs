using UnityEngine;
using Alteruna;
using TMPro;

public class VoteManager : AttributesSync
{
    [SynchronizableField] public int safeVotes = 0;
    [SynchronizableField] public int sacrificeVotes = 0;
    [SynchronizableField] public int relocatedbigvotes = 0;
    [SynchronizableField] public int relocatevotes = 0;
    [SynchronizableField] public int digitizevotes = 0;
    public TextMeshProUGUI safeText;
    public TextMeshProUGUI sacrificeText;
    public TextMeshProUGUI RelocateBigText;
    public TextMeshProUGUI RelocateText;
    public TextMeshProUGUI DigitizeText;

    [SerializeField] private GameObject[] votebuttons;

    public void addsafe()
    {
        safeVotes++;
        Commit();
        safeText.text = safeVotes.ToString();
    }
    public void addsacrifice()
    {
        sacrificeVotes++;
        Commit();
        sacrificeText.text = sacrificeVotes.ToString();
    }

    public void addRelocateBigVote()
    {
        relocatedbigvotes++;
        Commit();
        RelocateBigText.text = relocatevotes.ToString();
    }

    public void addRelocate()
    {
        relocatevotes++;
        Commit();
        RelocateText.text = relocatevotes.ToString();
    }

    public void adddigitize()
    {
        digitizevotes++;
        Commit();
        DigitizeText.text = digitizevotes.ToString();
    }

    public void ResetVotes()
    {
        digitizevotes = 0;
        relocatevotes = 0;
        relocatedbigvotes = 0;
        sacrificeVotes = 0;
        safeVotes = 0;

            foreach (GameObject button in votebuttons)
            {
                if (button != null)
                {
                    Interactme other = button.GetComponent<Interactme>();
                    if (other != null)
                    {
                        other.SetInteracted(false);
                        Debug.Log($"Enabled interaction on: {button.name}");
                    }
                }
            }
        Commit();
    }




    private void Update()
    {
        // Update UI every frame, disable this if you want secret votes that only show when you voted (maybe cool lol)
        safeText.text = safeVotes.ToString();
        sacrificeText.text = sacrificeVotes.ToString();
        DigitizeText.text = digitizevotes.ToString() ;
        RelocateText.text = relocatevotes.ToString();
        RelocateBigText.text = relocatedbigvotes.ToString();

    }
}
