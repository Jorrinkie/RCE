using UnityEngine;
using Alteruna;
using TMPro;

public class ResaurceManager : AttributesSync
{
    [Header("Resources (Synced)")]
    [SynchronizableField] public int money = 10;
    [SynchronizableField] public int manPower = 5;

    [Header("References")]
    [SerializeField] private MoneyViisualManager moneyVisualManager;
    [SerializeField] private ManPowerVisualManager manPowerVisualManager;

    [Header("UI References")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text manPowerText;

    private void Update()
    {
        // We updaten de display elke frame voor alle spelers zodat ze 
        // de gesynchroniseerde waarden direct zien.
        UpdateMoneyDisplay();
        UpdateManPowerDisplay();
    }

    // --- Logica die alleen door de Host wordt aangeroepen ---

    public void AddMoney(int amount)
    {
        money += amount;
        Commit(); // Stuurt nieuwe waarde naar iedereen
    }

    public void LoseMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
        Commit();
    }

    public void AddManPower(int amount)
    {
        manPower += amount;
        Commit();
    }

    public void LoseManPower(int amount)
    {
        manPower = Mathf.Max(0, manPower - amount);
        Commit();
    }

    // --- Display logica voor iedereen ---

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