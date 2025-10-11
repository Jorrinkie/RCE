using UnityEngine;
using UnityEngine.UI;

public class EnableOutline : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 5f;
    [SerializeField] private Image uiImageFull;
    private GameObject currentTarget;
    public GameObject CurrentTarget => currentTarget;
    public bool IsLookingAtHeritage { get; private set; } = false;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("Heritage") || hitObj.CompareTag("Button"))
            {
                if (currentTarget != hitObj)
                {
                    if (currentTarget != null)
                        SetOutline(currentTarget, false);

                    SetOutline(hitObj, true);
                    currentTarget = hitObj;
                }

                if (uiImageFull != null)
                    uiImageFull.gameObject.SetActive(true);

                IsLookingAtHeritage = true;
                return;
            }
        }

        if (currentTarget != null)
        {
            SetOutline(currentTarget, false);
            currentTarget = null;
        }

        if (uiImageFull != null)
            uiImageFull.gameObject.SetActive(false);

        IsLookingAtHeritage = false;
    }

    private void SetOutline(GameObject obj, bool enabled)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = enabled;
    }
}