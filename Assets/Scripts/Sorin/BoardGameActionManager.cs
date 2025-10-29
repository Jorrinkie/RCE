using UnityEngine;

public class BoardGameActionManager : MonoBehaviour
{
    [SerializeField] private ResaurceManager resourceManager;
    [SerializeField] private InfoDisplayManager infodisplaymanager;

    [SerializeField] private int leaveBehind_ManPowerCost = 0;
    [SerializeField] private int leaveBehind_MoneyCost = 0;

    [SerializeField] private int digitalize_ManPowerCost = 0;
    [SerializeField] private int digitalize_MoneyCost = 3;

    [SerializeField] private int investRepair_ManPowerCost = 2;
    [SerializeField] private int investRepair_MoneyCost = 2;

    [SerializeField] private int moveLocation_ManPowerCost = 3;
    [SerializeField] private int moveLocation_MoneyCost = 3;

    [SerializeField] private KeyCode leaveBehindKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode digitalizeKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode investRepairKey = KeyCode.Alpha3;
    [SerializeField] private KeyCode moveLocationKey = KeyCode.Alpha4;

    void Update()
    {
        if (Input.GetKeyDown(leaveBehindKey)) LeaveBehind();
        if (Input.GetKeyDown(digitalizeKey)) Digitalize();
        if (Input.GetKeyDown(investRepairKey)) InvestRepair();
        if (Input.GetKeyDown(moveLocationKey)) MoveLocation();
    }

    public void LeaveBehind()
    {
        if (resourceManager == null) return;
        resourceManager.LoseManPower(leaveBehind_ManPowerCost);
        resourceManager.LoseMoney(leaveBehind_MoneyCost);
    }

    public void Digitalize()
    {
        if (resourceManager == null) return;
        resourceManager.LoseManPower(digitalize_ManPowerCost);
        resourceManager.LoseMoney(digitalize_MoneyCost);
    }

    public void InvestRepair()
    {
        if (resourceManager == null) return;
        resourceManager.LoseManPower(investRepair_ManPowerCost);
        resourceManager.LoseMoney(investRepair_MoneyCost);
    }

    public void MoveLocation()
    {
        if (resourceManager == null) return;
        resourceManager.LoseManPower(moveLocation_ManPowerCost);
        resourceManager.LoseMoney(moveLocation_MoneyCost);
    }
}