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
    [SerializeField] private GameObject checkmark_Safe;
    [SerializeField] private GameObject checkmark_Sacrifice;
    [SerializeField] private GameObject checkmark_Digitize;
    [SerializeField] private GameObject checkmark_Relocate;
    [SerializeField] private GameObject checkmark_Adjust;

    [Header("Locations")]
    [SerializeField] private GameObject lighthouse;
    [SerializeField] private GameObject windmill;
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject church;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject tarneuzenStadhuis;

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
        // This remains LOCAL to each player to track their own checkmark
        public Dictionary<int, int> playerVotes = new Dictionary<int, int>();
    }

    private Dictionary<GameObject, LocationVotes> locationVotes = new Dictionary<GameObject, LocationVotes>();

    private void Start()
    {
        _multiplayer = FindObjectOfType<Multiplayer>();

        if (lighthouse != null) locationVotes[lighthouse] = new LocationVotes();
        if (windmill != null) locationVotes[windmill] = new LocationVotes();
        if (tower != null) locationVotes[tower] = new LocationVotes();
        if (church != null) locationVotes[church] = new LocationVotes();
        if (bridge != null) locationVotes[bridge] = new LocationVotes();
        if (tarneuzenStadhuis != null) locationVotes[tarneuzenStadhuis] = new LocationVotes();
    }

    private void Update()
    {
        GameObject currentLoc = GetActiveLocation();

        // ONLY Sync/Refresh when the location changes
        if (currentLoc != lastActiveLocation)
        {
            // 1. Save current totals to the old location before leaving
            if (lastActiveLocation != null) SyncToLocation(lastActiveLocation);

            lastActiveLocation = currentLoc;

            // 2. Load the totals for the new location
            SyncFromLocation(currentLoc);

            // 3. Update the local checkmarks
            UpdateCheckmarks();
        }

        // Host checks the vote count
        if (_multiplayer != null && _multiplayer.IsConnected && _multiplayer.Me.Index == 0)
        {
            CheckVoteCount();
        }

        UpdateUIStrings();
    }

    private void CheckVoteCount()
    {
        int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        int currentTotalVotes = safeVotes + sacrificeVotes + relocatedbigvotes + relocatevotes + digitizevotes;

        if (playerCount > 0 && currentTotalVotes >= playerCount)
        {
            if (!allVotesIn) { allVotesIn = true; Commit(); }
        }
        else if (allVotesIn)
        {
            allVotesIn = false; Commit();
        }
    }

    private GameObject GetActiveLocation()
    {
        if (lighthouse != null && lighthouse.activeSelf) return lighthouse;
        if (windmill != null && windmill.activeSelf) return windmill;
        if (tower != null && tower.activeSelf) return tower;
        if (church != null && church.activeSelf) return church;
        if (bridge != null && bridge.activeSelf) return bridge;
        if (tarneuzenStadhuis != null && tarneuzenStadhuis.activeSelf) return tarneuzenStadhuis;
        return null;
    }

    private void UpdateCheckmarks()
    {
        if (checkmark_Safe) checkmark_Safe.SetActive(false);
        if (checkmark_Sacrifice) checkmark_Sacrifice.SetActive(false);
        if (checkmark_Digitize) checkmark_Digitize.SetActive(false);
        if (checkmark_Relocate) checkmark_Relocate.SetActive(false);
        if (checkmark_Adjust) checkmark_Adjust.SetActive(false);

        GameObject loc = GetActiveLocation();
        if (loc == null || !locationVotes.ContainsKey(loc)) return;

        var v = locationVotes[loc];
        int myId = _multiplayer.Me.Index;

        if (v.playerVotes.TryGetValue(myId, out int myVote))
        {
            if (myVote == 0) checkmark_Safe.SetActive(true);
            else if (myVote == 1) checkmark_Sacrifice.SetActive(true);
            else if (myVote == 2) checkmark_Digitize.SetActive(true);
            else if (myVote == 3) checkmark_Relocate.SetActive(true);
            else if (myVote == 4) checkmark_Adjust.SetActive(true);
        }
    }

    private void SyncFromLocation(GameObject loc)
    {
        if (loc == null || !locationVotes.ContainsKey(loc)) return;
        var v = locationVotes[loc];

        safeVotes = v.safe;
        sacrificeVotes = v.sacrifice;
        relocatedbigvotes = v.relocateBig;
        relocatevotes = v.relocate;
        digitizevotes = v.digitize;
    }

    private void SyncToLocation(GameObject loc)
    {
        if (loc == null || !locationVotes.ContainsKey(loc)) return;
        var v = locationVotes[loc];

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
        GameObject loc = GetActiveLocation();
        if (loc == null) return;
        var v = locationVotes[loc];
        if (v.resolved) return;

        // LOCAL: Update history for checkmarks
        if (v.playerVotes.TryGetValue(playerId, out int oldVote))
            ModifyVote(oldVote, -1);

        // SYNC: Update the actual networked integers
        ModifyVote(vote, 1);
        v.playerVotes[playerId] = vote;

        UpdateCheckmarks();
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
        GameObject loc = GetActiveLocation();
        if (loc == null) return;

        safeVotes = 0;
        sacrificeVotes = 0;
        digitizevotes = 0;
        relocatevotes = 0;
        relocatedbigvotes = 0;
        allVotesIn = false;

        locationVotes[loc].resolved = false;
        locationVotes[loc].playerVotes.Clear();

        UpdateCheckmarks();
        Commit();

        if (audioSource && resetVotesSound) audioSource.PlayOneShot(resetVotesSound);
    }

    public bool IsLocationResolved(GameObject loc)
    {
        if (loc != null && locationVotes.ContainsKey(loc))
        {
            return locationVotes[loc].resolved;
        }
        return false;
    }


    public void FindHighest()
    {
        int highest = Mathf.Max(safeVotes, sacrificeVotes, relocatedbigvotes, relocatevotes, digitizevotes);
        int tieCount = (safeVotes == highest ? 1 : 0) + (sacrificeVotes == highest ? 1 : 0) + (relocatedbigvotes == highest ? 1 : 0) + (relocatevotes == highest ? 1 : 0) + (digitizevotes == highest ? 1 : 0);

        if (tieCount > 1 || highest <= 0) { ResetVotes(); return; }

        safeWon = safeVotes == highest;
        sacrificeWon = sacrificeVotes == highest;
        relocateBigWon = relocatedbigvotes == highest;
        relocateWon = relocatevotes == highest;
        digitizeWon = digitizevotes == highest;

        GameObject loc = GetActiveLocation();
        if (loc != null) locationVotes[loc].resolved = true;

        Commit();

        if (audioSource && highestVoteSound) audioSource.PlayOneShot(highestVoteSound);

        if (safeWon) MoneyManager.InvestRepair();
        else if (sacrificeWon) MoneyManager.LeaveBehind();
        else if (relocateBigWon || relocateWon) MoneyManager.RelocateSmall();
        else if (digitizeWon) MoneyManager.Digitalize();
    }
}