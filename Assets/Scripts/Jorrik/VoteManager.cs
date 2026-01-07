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

    // Nieuwe gesynchroniseerde variabele
    [SynchronizableField] public bool allVotesIn = false;

    public TextMeshProUGUI safeText;
    public TextMeshProUGUI sacrificeText;
    public TextMeshProUGUI RelocateBigText;
    public TextMeshProUGUI RelocateText;
    public TextMeshProUGUI DigitizeText;

    [SerializeField] private GameObject[] votebuttons;

    // Helper om het totaal aantal stemmen te berekenen
    public int GetTotalVotes()
    {
        return safeVotes + sacrificeVotes + relocatedbigvotes + relocatevotes + digitizevotes;
    }

    private void Update()
    {
        // UI bijwerken
        UpdateUIStrings();


        CheckVoteCount();

        // Reset logica (jouw bestaande code)
        if (GetTotalVotes() == 0 && allVotesIn)
        {
            allVotesIn = false;
            EnableButtonsLocally();
            Commit();
        }
    }

    private void UpdateUIStrings()
    {
        safeText.text = safeVotes.ToString();
        sacrificeText.text = sacrificeVotes.ToString();
        DigitizeText.text = digitizevotes.ToString();
        RelocateText.text = relocatevotes.ToString();
        RelocateBigText.text = relocatedbigvotes.ToString();
    }

    private void CheckVoteCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playerCount = players.Length;
        int currentTotalVotes = GetTotalVotes();

        // Als het aantal stemmen gelijk is aan spelers (en er zijn spelers)
        if (playerCount > 0 && currentTotalVotes >= playerCount)
        {
            if (!allVotesIn)
            {
                allVotesIn = true;
                Commit();
                Debug.Log("Alle stemmen zijn binnen!");
            }
        }
        else
        {
            if (allVotesIn)
            {
                allVotesIn = false;
                Commit();
            }
        }
    }

    // --- Jouw bestaande Add methodes ---
    public void addsafe() { safeVotes++; Commit(); }
    public void addsacrifice() { sacrificeVotes++; Commit(); }
    public void addRelocateBigVote() { relocatedbigvotes++; Commit(); }
    public void addRelocate() { relocatevotes++; Commit(); }
    public void adddigitize() { digitizevotes++; Commit(); }

    public void ResetVotes()
    {
        digitizevotes = 0;
        relocatevotes = 0;
        relocatedbigvotes = 0;
        sacrificeVotes = 0;
        safeVotes = 0;
        allVotesIn = false;

        EnableButtonsLocally();
        Commit();
    }

    private void EnableButtonsLocally()
    {
        foreach (GameObject button in votebuttons)
        {
            if (button != null)
            {
                Interactme other = button.GetComponent<Interactme>();
                if (other != null) other.SetInteracted(false);
            }
        }
    }
}