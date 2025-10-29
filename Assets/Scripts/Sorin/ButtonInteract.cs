using UnityEngine;
using System.Collections;

public class ButtonInteract : MonoBehaviour
{

    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private BoardGameActionManager boardgameactionmanager;
    [SerializeField] private ResaurceManager resaurcemanager;
    public enum ActionType
    {
        None,
        LeaveBehind,
        Digitalize,
        InvestRepair,
        RelocateSmall,
        RelocateBig
            
    }

    [SerializeField] private ActionType actionToTrigger = ActionType.None;

    private bool interacted = false;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            float range = 3f;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                if (hit.transform == transform)
                    Interact();
            }
        }
    }

    public void Interact()
    {
        if (interacted) return;
        interacted = true;

        StartCoroutine(PlayButtonDownAnimation());
        Invoke(nameof(ExecuteAction), 0.2f);
    }

    private IEnumerator PlayButtonDownAnimation()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 pressedPos = startPos + new Vector3(0f, -0.21f, 0f);
        float duration = 0.2f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            transform.localPosition = Vector3.Lerp(startPos, pressedPos, progress);
            yield return null;
        }



        // ACTIVATE FOR REUTRN OF BUTTON

        /*
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            transform.localPosition = Vector3.Lerp(pressedPos, startPos, progress);
            yield return null;
        }

        transform.localPosition = startPos;
        */
    }

    private void ExecuteAction()
    {
        if (boardgameactionmanager == null) return;

        switch (actionToTrigger)
        {
            case ActionType.LeaveBehind:
                boardgameactionmanager.LeaveBehind();
                break;
            case ActionType.Digitalize:
                boardgameactionmanager.Digitalize();
                break;
            case ActionType.InvestRepair:
                boardgameactionmanager.InvestRepair();
                break;
            case ActionType.RelocateBig:
                boardgameactionmanager.RelocateSmall();
                break;
            case ActionType.RelocateSmall:
                boardgameactionmanager.RelocateBig();
                break;

        }


        if (resaurcemanager != null)
        {
            resaurcemanager.UpdateMoneyDisplay();
            resaurcemanager.UpdateManPowerDisplay();
        }
    }
}

