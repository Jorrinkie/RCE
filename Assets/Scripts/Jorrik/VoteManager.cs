using UnityEngine;
using Alteruna;
using TMPro;
using System.Collections.Generic;

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

    [Header("Locations")]
    [SerializeField] private GameObject lighthouse;
    [SerializeField] private GameObject windmill;
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject church;
    [SerializeField] private GameObject bridge;

    [Header("Results")]
    [SynchronizableField] public bool safeWon = false;
    [SynchronizableField] public bool sacrificeWon = false;
    [SynchronizableField] public bool relocateBigWon = false;
    [SynchronizableField] public bool relocateWon = false;
    [SynchronizableField] public bool digitizeWon = false;

    [SerializeField] private BoardGameActionManager MoneyManager;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resetVotesSound;
    [SerializeField] private AudioClip highestVoteSound;

    private Multiplayer _multiplayer;

    [System.Serializable]
    private class LocationVotes
    {
        public int safe;
        public int sacrifice;
        public int relocateBig;
        public int relocate;
        public int digitize;
        public bool resolved;

        // Each location now remembers who voted for what locally
        public Dictionary<int, int> playerVotes = new Dictionary<int, int>();
    }

    private Dictionary<GameObject, LocationVotes> locationVotes = new Dictionary<GameObject, LocationVotes>();
    private Dictionary<int, int> playerVotes = new Dictionary<int, int>();

    private void Start()
    {
        _multiplayer = FindObjectOfType<Multiplayer>();

        locationVotes[lighthouse] = new LocationVotes();
        locationVotes[windmill] = new LocationVotes();
        locationVotes[tower] = new LocationVotes();
        locationVotes[church] = new LocationVotes();
        locationVotes[bridge] = new LocationVotes();
    }

    private void Update()
    {
        SyncFromLocation();
        UpdateUIStrings();
    }

    private GameObject GetActiveLocation()
    {
        if (lighthouse.activeSelf) return lighthouse;
        if (windmill.activeSelf) return windmill;
        if (tower.activeSelf) return tower;
        if (church.activeSelf) return church;
        if (bridge.activeSelf) return bridge;
        return null;
    }

    private LocationVotes CurrentVotes
    {
        get
        {
            GameObject loc = GetActiveLocation();
            if (loc == null) return null;
            return locationVotes[loc];
        }
    }

    private void SyncFromLocation()
    {
        var v = CurrentVotes;
        if (v == null) return;

        safeVotes = v.safe;
        sacrificeVotes = v.sacrifice;
        relocatedbigvotes = v.relocateBig;
        relocatevotes = v.relocate;
        digitizevotes = v.digitize;
    }

    private void SyncToLocation()
    {
        var v = CurrentVotes;
        if (v == null) return;

        v.safe = safeVotes;
        v.sacrifice = sacrificeVotes;
        v.relocateBig = relocatedbigvotes;
        v.relocate = relocatevotes;
        v.digitize = digitizevotes;
    }

    private void UpdateUIStrings()
    {
        if (safeText) safeText.text = safeVotes.ToString();
        if (sacrificeText) sacrificeText.text = sacrificeVotes.ToString();
        if (DigitizeText) DigitizeText.text = digitizevotes.ToString();
        if (RelocateText) RelocateText.text = relocatevotes.ToString();
        if (RelocateBigText) RelocateBigText.text = relocatedbigvotes.ToString();
    }

    public void RegisterVote(int playerId, int vote)
    {
        var v = CurrentVotes;
        if (v == null || v.resolved) return;

        // Look for the player's vote ONLY within this specific location's data
        if (v.playerVotes.TryGetValue(playerId, out int oldVote))
        {
            // Only subtract if they actually voted here before
            ModifyVote(oldVote, -1);
        }

        ModifyVote(vote, 1);
        v.playerVotes[playerId] = vote; // Save the vote to this location's history

        SyncToLocation();
        Commit();
    }

    private void ModifyVote(int vote, int amount)
    {
        if (vote == 0) safeVotes += amount;
        else if (vote == 1) sacrificeVotes += amount;
        else if (vote == 2) digitizevotes += amount;
        else if (vote == 3) relocatevotes += amount;
        else if (vote == 4) relocatedbigvotes += amount;
    }

    public void ResetVotes()
    {
        var v = CurrentVotes;
        if (v == null) return;

        safeVotes = 0;
        sacrificeVotes = 0;
        digitizevotes = 0;
        relocatevotes = 0;
        relocatedbigvotes = 0;

        v.resolved = false;
        v.playerVotes.Clear(); // Clear the specific location's history

        SyncToLocation();
        Commit();

        if (audioSource && resetVotesSound)
            audioSource.PlayOneShot(resetVotesSound);
    }

    public void FindHighest()
    {
        var v = CurrentVotes;
        if (v == null || v.resolved) return;

        int highest = Mathf.Max(safeVotes, sacrificeVotes, relocatedbigvotes, relocatevotes, digitizevotes);

        int tie =
            (safeVotes == highest ? 1 : 0) +
            (sacrificeVotes == highest ? 1 : 0) +
            (relocatedbigvotes == highest ? 1 : 0) +
            (relocatevotes == highest ? 1 : 0) +
            (digitizevotes == highest ? 1 : 0);

        if (tie > 1)
        {
            ResetVotes();
            return;
        }

        safeWon = safeVotes == highest;
        sacrificeWon = sacrificeVotes == highest;
        relocateBigWon = relocatedbigvotes == highest;
        relocateWon = relocatevotes == highest;
        digitizeWon = digitizevotes == highest;

        v.resolved = true;
        SyncToLocation();
        Commit();

        if (audioSource && highestVoteSound)
            audioSource.PlayOneShot(highestVoteSound);

        if (safeWon) MoneyManager.InvestRepair();
        if (sacrificeWon) MoneyManager.LeaveBehind();
        if (relocateBigWon) MoneyManager.RelocateSmall();
        if (relocateWon) MoneyManager.RelocateSmall();
        if (digitizeWon) MoneyManager.Digitalize();
    }
}