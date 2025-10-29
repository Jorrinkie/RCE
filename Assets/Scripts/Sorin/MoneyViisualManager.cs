using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyViisualManager : MonoBehaviour
{
    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject stackPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform stackParent;
    [SerializeField] private Transform coinParent;

    [Header("Config")]
    [SerializeField] private int maxStacks = 30;
    [SerializeField] private int stackLimit = 15;
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float animSpeed = 0.15f;

    [Header("Layout")]
    [SerializeField] private Vector3 stackOffset = new Vector3(0, 0.5f, 0); // vertical offset
    [SerializeField] private Vector3 stackColumnOffset = new Vector3(1.0f, 0, 0); // horizontal offset for columns
    [SerializeField] private Vector3 coinOffset = new Vector3(0, 0.3f, 0); // vertical
    [SerializeField] private Vector3 coinColumnOffset = new Vector3(0.6f, 0, 0); // horizontal

    private List<GameObject> stackPool = new List<GameObject>();
    private List<GameObject> coinPool = new List<GameObject>();
    private int currentDisplayedMoney = 0;

    void Start()
    {
        for (int i = 0; i < maxStacks; i++)
        {
            var stack = Instantiate(stackPrefab, stackParent);
            stack.SetActive(false);
            stackPool.Add(stack);
        }

        for (int i = 0; i < maxStacks; i++)
        {
            var coin = Instantiate(coinPrefab, coinParent);
            coin.SetActive(false);
            coinPool.Add(coin);
        }
    }

    public void UpdateMoneyVisual(int newMoney)
    {
        if (newMoney == currentDisplayedMoney) return;
        StartCoroutine(AnimateMoneyChange(currentDisplayedMoney, newMoney));
        currentDisplayedMoney = newMoney;
    }

    private IEnumerator AnimateMoneyChange(int oldAmount, int newAmount)
    {
        bool increasing = newAmount > oldAmount;
        int step = increasing ? 1 : -1;

        for (int i = oldAmount; i != newAmount; i += step)
        {
            int stacksToShow = Mathf.Min(i + step, stackLimit);
            int coinsToShow = Mathf.Max(0, (i + step) - stackLimit);

            ArrangeStacks(stacksToShow);
            ArrangeCoins(coinsToShow);

            GameObject popped = (i + step <= stackLimit)
                ? stackPool[Mathf.Clamp(i + step - 1, 0, stackPool.Count - 1)]
                : coinPool[Mathf.Clamp((i + step) - stackLimit - 1, 0, coinPool.Count - 1)];

            if (popped.activeSelf)
                StartCoroutine(PopEffect(popped.transform));

            yield return new WaitForSeconds(animSpeed);
        }
    }

    private void ArrangeStacks(int count)
    {
        for (int i = 0; i < stackPool.Count; i++)
        {
            if (i < count)
            {
                if (!stackPool[i].activeSelf)
                {
                    stackPool[i].SetActive(true);

                    int column = i / 5;
                    int row = i % 5;
                    stackPool[i].transform.localPosition = stackColumnOffset * column + stackOffset * row;
                }
            }
            else
            {
                stackPool[i].SetActive(false);
            }
        }
    }

    private void ArrangeCoins(int count)
    {
        for (int i = 0; i < coinPool.Count; i++)
        {
            if (i < count)
            {
                if (!coinPool[i].activeSelf)
                {
                    coinPool[i].SetActive(true);
                    Vector3 pos = Vector3.zero;

                    if (i < 2) pos = coinOffset * i; 
                    else if (i < 4) pos = coinColumnOffset + coinOffset * (i - 2); 
                    else if (i == 4) pos = coinColumnOffset * 0.5f + coinOffset * 0; 

                    coinPool[i].transform.localPosition = pos;
                }
            }
            else
            {
                coinPool[i].SetActive(false);
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