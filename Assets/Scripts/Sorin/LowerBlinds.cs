using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowerBlinds : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float distance = 4f;

    private bool isMoving = false;

    public void ChangeHeritage(List<GameObject> toDeactivate, GameObject toActivate)
    {
        if (!isMoving)
            StartCoroutine(LowerSwapLiftRoutine(toDeactivate, toActivate));
    }

    private IEnumerator LowerSwapLiftRoutine(List<GameObject> objectsToDeactivate, GameObject objectToActivate)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 loweredPos = startPos - new Vector3(0, distance, 0);
        float elapsed = 0f;

        // Lower blinds
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, loweredPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = loweredPos;

        // Swap environment
        if (objectsToDeactivate != null)
        {
            foreach (var obj in objectsToDeactivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        // Raise blinds
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(loweredPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        isMoving = false;
    }
}
