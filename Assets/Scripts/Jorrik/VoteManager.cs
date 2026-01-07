using UnityEngine;
using Alteruna;
using TMPro;

public class VoteManager : AttributesSync
{
    [Header("Sync Variables")]
    [SynchronizableField] public int safeVotes = 0;
    [SynchronizableField] public int sacrificeVotes = 0;
    [SynchronizableField] public int relocatedbigvotes = 0;
    [SynchronizableField] public int relocatevotes = 0;
    [SynchronizableField] public int digitizevotes = 0;
    [SynchronizableField] public bool allVotesIn = false;

    [Header("UI References")]
    public TextMeshProUGUI safeText;
    public TextMeshProUGUI sacrificeText;
    public TextMeshProUGUI RelocateBigText;
    public TextMeshProUGUI RelocateText;
    public TextMeshProUGUI DigitizeText;

    [Header("Settings")]
    [SerializeField] private GameObject[] votebuttons;

    [Header("Results")]
    [SynchronizableField] public bool safeWon = false;
    [SynchronizableField] public bool sacrificeWon = false;
    [SynchronizableField] public bool relocateBigWon = false;
    [SynchronizableField] public bool relocateWon = false;
    [SynchronizableField] public bool digitizeWon = false;

    [SerializeField] private BoardGameActionManager MoneyManager;


    // Referentie naar de centrale Multiplayer component
    private Multiplayer _multiplayer;
    private bool _wasReset = false;

    private void Start()
    {
        // Zoek de Multiplayer component in de scene
        _multiplayer = FindObjectOfType<Multiplayer>();

        if (_multiplayer == null)
        {
            Debug.LogError("Geen Multiplayer component gevonden in de scene!");
        }
    }

    private void Update()
    {
        UpdateUIStrings();

        // 1. Check of we de Host zijn
        // In Alteruna is de Host degene die de Room beheert
        if (_multiplayer != null && _multiplayer.IsConnected)
        {
            // De host check op de meest universele manier:
            if (_multiplayer.Me.Index == 0)
            {
                CheckVoteCount();
            }
        }

        // 2. De Reset Check (werkt voor iedereen)
        if (GetTotalVotes() == 0)
        {
            if (!_wasReset)
            {
                EnableButtonsLocally();
                _wasReset = true;
            }
        }
        else
        {
            _wasReset = false;
        }
    }

    public int GetTotalVotes()
    {
        return safeVotes + sacrificeVotes + relocatedbigvotes + relocatevotes + digitizevotes;
    }

    private void UpdateUIStrings()
    {
        if (safeText) safeText.text = safeVotes.ToString();
        if (sacrificeText) sacrificeText.text = sacrificeVotes.ToString();
        if (DigitizeText) DigitizeText.text = digitizevotes.ToString();
        if (RelocateText) RelocateText.text = relocatevotes.ToString();
        if (RelocateBigText) RelocateBigText.text = relocatedbigvotes.ToString();
    }

    private void CheckVoteCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playerCount = players.Length;
        int currentTotalVotes = GetTotalVotes();

        if (playerCount > 0 && currentTotalVotes >= playerCount)
        {
            if (!allVotesIn)
            {
                allVotesIn = true;
                Commit();
            }
        }
        else if (allVotesIn)
        {
            allVotesIn = false;
            Commit();
        }
    }

    public void addsafe() { safeVotes++; Commit(); }
    public void addsacrifice() { sacrificeVotes++; Commit(); }
    public void addRelocateBigVote() { relocatedbigvotes++; Commit(); }
    public void addRelocate() { relocatevotes++; Commit(); }
    public void adddigitize() { digitizevotes++; Commit(); }

    public void ResetVotes()
    {
        // Iedereen mag de reset aanvragen, maar we synchroniseren het resultaat
        digitizevotes = 0;
        relocatevotes = 0;
        relocatedbigvotes = 0;
        sacrificeVotes = 0;
        safeVotes = 0;
        allVotesIn = false;
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

    public void FindHighest()
    {
        // 1. Reset eerst alle uitslagen naar false
        safeWon = false;
        sacrificeWon = false;
        relocateBigWon = false;
        relocateWon = false;
        digitizeWon = false;

        // 2. Vind het hoogste getal van alle stemmen
        int highest = Mathf.Max(safeVotes, sacrificeVotes, relocatedbigvotes, relocatevotes, digitizevotes);

        // 3. Check of er wel gestemd is
        if (highest <= 0)
        {
            Debug.Log("[Vote Result] Geen stemmen uitgebracht.");
            Commit();
            return;
        }

        // --- NIEUW: TIE-BREAKER LOGICA ---
        int tieCount = 0;
        if (safeVotes == highest) tieCount++;
        if (sacrificeVotes == highest) tieCount++;
        if (relocatedbigvotes == highest) tieCount++;
        if (relocatevotes == highest) tieCount++;
        if (digitizevotes == highest) tieCount++;

        // Als er meer dan 1 categorie de hoogste score heeft, is het gelijkspel
        if (tieCount > 1)
        {
            Debug.Log($"[Vote Result] Gelijkspel gedetecteerd! ({tieCount} opties met {highest} stemmen). Stemmen worden gereset.");
            ResetVotes(); // Gebruik je bestaande reset functie
            return;
        }
        // --------------------------------

        string winnaarNaam = "";

        // 4. Bepaal wie de winnaar is (hier komen we alleen als er geen gelijkspel is)
        if (safeVotes == highest)
        {
            safeWon = true;
            winnaarNaam = "Safe";
        }
        else if (sacrificeVotes == highest)
        {
            sacrificeWon = true;
            winnaarNaam = "Sacrifice";
        }
        else if (relocatedbigvotes == highest)
        {
            relocateBigWon = true;
            winnaarNaam = "Relocate Big";
        }
        else if (relocatevotes == highest)
        {
            relocateWon = true;
            winnaarNaam = "Relocate";
        }
        else if (digitizevotes == highest)
        {
            digitizeWon = true;
            winnaarNaam = "Digitize";
        }

        Commit();
        Debug.Log($"[Vote Result] Stemming Gesloten! Winnaar: {winnaarNaam} met {highest} stemmen.");



        if (safeWon)
        {
            MoneyManager.InvestRepair();
        }
        if (sacrificeWon)
        {
            MoneyManager.LeaveBehind();
        }
        if (relocateBigWon)
        {
            MoneyManager.RelocateSmall();
        }
        if (relocateWon)
        {
            MoneyManager.RelocateSmall();
        }
        if (digitizeWon)
        {
            MoneyManager.Digitalize();
        }
    }
}