using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    [SerializeField] private Animation animationComponent;
    [SerializeField] private string clipName = "ButtonPress";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [SerializeField] private BoardGameActionManager boardgameactionmanager;

    public enum ActionType
    {
        None,
        LeaveBehind,
        Digitalize,
        InvestRepair,
        MoveLocation
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

        if (animationComponent != null && !string.IsNullOrEmpty(clipName))
            animationComponent.Play(clipName);

        interacted = true;

        Invoke(nameof(ExecuteAction), 0.2f);

      
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
            case ActionType.MoveLocation:
                boardgameactionmanager.MoveLocation();
                break;
        }
    }
}

