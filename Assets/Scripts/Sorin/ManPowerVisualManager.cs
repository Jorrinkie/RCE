using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManPowerVisualManager : MonoBehaviour
{
    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform unitParent;

    [Header("Config")]
    [SerializeField] private int maxUnits = 20;
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float animSpeed = 0.1f;

    [Header("Layout")]
    [SerializeField] private int columns = 5;  // how many per row
    [SerializeField] private Vector3 gridSpacing = new Vector3(0.6f, 0.6f, 0); // X = horizontal spacing, Y = vertical

    private List<GameObject> unitPool = new List<GameObject>();
    private int currentDisplayedManPower = 0;

    void Start()
    {
        for (int i = 0; i < maxUnits; i++)
        {
            var unit = Instantiate(unitPrefab, unitParent);
            unit.SetActive(false);
            unitPool.Add(unit);
        }
    }

    public void UpdateManPowerVisual(int newAmount)
    {
        if (newAmount == currentDisplayedManPower) return;
        StartCoroutine(AnimateManPowerChange(currentDisplayedManPower, newAmount));
        currentDisplayedManPower = newAmount;
    }

    private IEnumerator AnimateManPowerChange(int oldAmount, int newAmount)
    {
        bool increasing = newAmount > oldAmount;
        int step = increasing ? 1 : -1;

        for (int i = oldAmount; i != newAmount; i += step)
        {
            int next = i + step;
            ArrangeUnits(next);

            if (next > 0 && next <= unitPool.Count)
            {
                GameObject popped = unitPool[next - 1];
                if (popped.activeSelf)
                    StartCoroutine(PopEffect(popped.transform));
            }

            yield return new WaitForSeconds(animSpeed);
        }
    }

    private void ArrangeUnits(int count)
    {
        for (int i = 0; i < unitPool.Count; i++)
        {
            if (i < count)
            {
                if (!unitPool[i].activeSelf)
                {
                    unitPool[i].SetActive(true);
                }

                int row = i / columns;
                int col = i % columns;

                unitPool[i].transform.localPosition =
                    new Vector3(col * gridSpacing.x, -row * gridSpacing.y, 0);
            }
            else
            {
                unitPool[i].SetActive(false);
            }
        }
    }

    private IEnumerator PopEffect(Transform target)
    {
        Vector3 startScale = Vector3.one;
        Vector3 peakScale = Vector3.one * popScale;
        float t = 0f;

        while (t < 1f)
        {
            target.localScale = Vector3.Lerp(startScale, peakScale, t);
            t += Time.deltaTime * 8f;
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            target.localScale = Vector3.Lerp(peakScale, startScale, t);
            t += Time.deltaTime * 8f;
            yield return null;
        }

        target.localScale = startScale;
    }
}
