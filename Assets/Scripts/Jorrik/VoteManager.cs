using UnityEngine;
using Alteruna;
using TMPro;

public class VoteManager : AttributesSync
{
    [SynchronizableField] public int safeVotes = 0;
    [SynchronizableField] public int sacrificeVotes = 0;
    public TextMeshProUGUI safeText;
    public TextMeshProUGUI sacrificeText;

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

        private void Update()
    {
        // Update UI every frame, disable this if you want secret votes that only show when you voted (maybe cool lol)
        safeText.text = safeVotes.ToString();
        sacrificeText.text = sacrificeVotes.ToString();
    }
}
