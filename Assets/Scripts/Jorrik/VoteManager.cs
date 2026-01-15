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

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI safeText;
    [SerializeField] private TextMeshProUGUI sacrificeText;
    [SerializeField] private TextMeshProUGUI RelocateBigText;
    [SerializeField] private TextMeshProUGUI RelocateText;
    [SerializeField] private TextMeshProUGUI DigitizeText;

    [Header("Choice Checkmarks")]
    [SerializeField] private GameObject checkmark_Safe;      // Index 0
    [SerializeField] private GameObject checkmark_Sacrifice;  // Index 1
    [SerializeField] private GameObject checkmark_Digitize;   // Index 2
    [SerializeField] private GameObject checkmark_Relocate;   // Index 3
    [SerializeField] private GameObject checkmark_Adjust;     // Index 4

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
    private GameObject lastActiveLocation;

    [System.Serializable]
    private class LocationVotes
    {
        public int safe;
        public int sacrifice;
        public int relocateBig;
        public int relocate;
        public int digitize;
        public bool resolved;
        // Specific votes for players AT THIS location
        public Dictionary<int, int> playerVotes = new Dictionary<int, int>();
    }

    private Dictionary<GameObject, LocationVotes> locationVotes = new Dictionary<GameObject, LocationVotes>();

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
        GameObject currentLoc = GetActiveLocation();

        // If we switched from Lighthouse to Windmill, refresh the UI
        if (currentLoc != lastActiveLocation)
        {
            lastActiveLocation = currentLoc;
            UpdateCheckmarks();
        }

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

    private void UpdateCheckmarks()
    {
        // Hide all checkmarks first
        if (checkmark_Safe) checkmark_Safe.SetActive(false);
        if (checkmark_Sacrifice) checkmark_Sacrifice.SetActive(false);
        if (checkmark_Digitize) checkmark_Digitize.SetActive(false);
        if (checkmark_Relocate) checkmark_Relocate.SetActive(false);
        if (checkmark_Adjust) checkmark_Adjust.SetActive(false);

        var v = CurrentVotes;
        if (v == null || _multiplayer == null) return;

        int myId = _multiplayer.Me.Index;

        // Check if I have a vote recorded for the CURRENT active object
        if (v.playerVotes.TryGetValue(myId, out int myVote))
        {
            if (myVote == 0 && checkmark_Safe) checkmark_Safe.SetActive(true);
            else if (myVote == 1 && checkmark_Sacrifice) checkmark_Sacrifice.SetActive(true);
            else if (myVote == 2 && checkmark_Digitize) checkmark_Digitize.SetActive(true);
            else if (myVote == 3 && checkmark_Relocate) checkmark_Relocate.SetActive(true);
            else if (myVote == 4 && checkmark_Adjust) checkmark_Adjust.SetActive(true);
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

        if (v.playerVotes.TryGetValue(playerId, out int oldVote))
            ModifyVote(oldVote, -1);

        ModifyVote(vote, 1);
        v.playerVotes[playerId] = vote;

        SyncToLocation();
        UpdateCheckmarks(); // Refresh UI as soon as we vote
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
        v.playerVotes.Clear();

        SyncToLocation();
        UpdateCheckmarks();
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