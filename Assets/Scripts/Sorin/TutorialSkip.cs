using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialSkip : MonoBehaviour, IInteractable
{
    public enum ButtonAction
    {
        SkipTutorial,
        PlayGame
    }

    [SerializeField] private TutorialController tutorialController;
    [SerializeField] private TMP_Text voteText;
    [SerializeField] private Transform buttonModel;
    [SerializeField] private ButtonAction action = ButtonAction.SkipTutorial;

    [Header("Press Settings")]
    [SerializeField] private float pressDepth = 0.2f;   
    [SerializeField] private float pressSpeed = 5f;     

    private static int skipVotes;
    private static int playVotes;

    private bool hasVoted;
    private Vector3 originalPos;
    private Vector3 targetPos;

    private int PlayerCount => GameObject.FindGameObjectsWithTag("Player").Length;

    private void Start()
    {
        if (buttonModel != null)
        {
            originalPos = buttonModel.localPosition;
            targetPos = originalPos;
        }

        UpdateVoteText();
    }

    private void Update()
    {
        if (buttonModel != null)
        {
            buttonModel.localPosition = Vector3.Lerp(buttonModel.localPosition, targetPos, Time.deltaTime * pressSpeed);
        }
    }

    public void Interact()
    {
        PressButton();
    }

    private void PressButton()
    {
        if (hasVoted) return;

        hasVoted = true;

        if (buttonModel != null)
            targetPos = originalPos + new Vector3(0, -pressDepth, 0);

        switch (action)
        {
            case ButtonAction.SkipTutorial:
                skipVotes++;
                UpdateVoteText(skipVotes);
                if (skipVotes >= PlayerCount)
                    tutorialController.SkipTutorial();
                break;

            case ButtonAction.PlayGame:
                playVotes++;
                UpdateVoteText(playVotes);
                if (playVotes >= PlayerCount)
                    tutorialController.PlayButtonPressed();
                break;
        }
    }

    private void UpdateVoteText(int votes)
    {
        if (voteText != null)
            voteText.text = votes + " / " + PlayerCount + " voted";
    }

    private void UpdateVoteText()
    {
        int votes = action == ButtonAction.SkipTutorial ? skipVotes : playVotes;
        UpdateVoteText(votes);
    }
}