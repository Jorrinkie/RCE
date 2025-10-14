using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ResaurceManager : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private int money = 10;
    [SerializeField] private int manPower = 5;

    [Header("References")]
    [SerializeField] private MoneyViisualManager moneyVisualManager;
    [SerializeField] private ManPowerVisualManager manPowerVisualManager;

    [Header("UI References")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text manPowerText;

    [Header("Debug / Settings")]
    [SerializeField] private KeyCode loseMoneyKey = KeyCode.T;
    [SerializeField] private KeyCode getMoneyKey = KeyCode.Y;
    [SerializeField] private int moneyLossPerPress = 1;
    [SerializeField] private int moneyGainPerPress = 1;

    void Start()
    {
        UpdateMoneyDisplay();
        UpdateManPowerDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(loseMoneyKey))
        {
            LoseMoney(moneyLossPerPress);
            LoseManPower(moneyLossPerPress);
        }

        if (Input.GetKeyDown(getMoneyKey))
        {
            AddMoney(moneyGainPerPress);
            AddManPower(moneyGainPerPress);
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyDisplay();
    }

    public void LoseMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
        UpdateMoneyDisplay();
    }

    public void AddManPower(int amount)
    {
        manPower += amount;
        UpdateManPowerDisplay();
    }

    public void LoseManPower(int amount)
    {
        manPower = Mathf.Max(0, manPower - amount);
        UpdateManPowerDisplay();
    }

    public void UpdateMoneyDisplay()
    {
        if (moneyVisualManager != null)
            moneyVisualManager.UpdateMoneyVisual(money);

        if (moneyText != null)
            moneyText.text = "Money: " + money;
    }

    public void UpdateManPowerDisplay()
    {
        if (manPowerVisualManager != null)
            manPowerVisualManager.UpdateManPowerVisual(manPower);

        if (manPowerText != null)
            manPowerText.text = "Manpower: " + manPower;
    }

    public int GetMoney() => money;
    public int GetManPower() => manPower;
}