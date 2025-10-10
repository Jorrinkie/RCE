using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResaurceManager : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private int money = 10;
    [SerializeField] private int manPower = 5;

    [Header("References")]
    [SerializeField] private MoneyViisualManager moneyVisualManager;
    [SerializeField] private ManPowerVisualManager manPowerVisualManager;

    [Header("Debug / Settings")]
    [SerializeField] private KeyCode loseMoneyKey = KeyCode.T;
    [SerializeField] private KeyCode getMoneyKey = KeyCode.Y;
    [SerializeField] private int moneyLossPerPress = 1;
    [SerializeField] private int moneyGainPerPress = 1;

    void Start()
    {
        if (moneyVisualManager != null)
            moneyVisualManager.UpdateMoneyVisual(money);

        if (manPowerVisualManager != null)
            manPowerVisualManager.UpdateManPowerVisual(manPower);
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
        if (manPowerVisualManager != null)
            manPowerVisualManager.UpdateManPowerVisual(manPower);
    }

    public void LoseManPower(int amount)
    {
        manPower = Mathf.Max(0, manPower - amount);
        if (manPowerVisualManager != null)
            manPowerVisualManager.UpdateManPowerVisual(manPower);
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyVisualManager != null)
            moneyVisualManager.UpdateMoneyVisual(money);
    }

    private void UpdateManPowerDisplay()
    {
        if (manPowerVisualManager != null)
            manPowerVisualManager.UpdateManPowerVisual(money);
    }

 
    public int GetMoney() => money;
    public int GetManPower() => manPower;
}
