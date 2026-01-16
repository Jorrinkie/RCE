using UnityEngine;
using Alteruna;

public class BoardGameActionManager : MonoBehaviour
{
    [SerializeField] private ResaurceManager resourceManager;
    private Multiplayer _multiplayer;

    [Header("Action Costs")]
    [SerializeField] private int leaveBehind_ManPowerCost = 0;
    [SerializeField] private int leaveBehind_MoneyCost = 0;

    [SerializeField] private int digitalize_ManPowerCost = 0;
    [SerializeField] private int digitalize_MoneyCost = 3;

    [SerializeField] private int investRepair_ManPowerCost = 2;
    [SerializeField] private int investRepair_MoneyCost = 2;

    [SerializeField] private int relocateSmall_ManPowerCost = 3;
    [SerializeField] private int relocateSmall_MoneyCost = 3;

    [SerializeField] private int relocateBig_ManPowerCost = 3;
    [SerializeField] private int relocateBig_MoneyCost = 3;

    private void Start()
    {
        _multiplayer = FindObjectOfType<Multiplayer>();
    }

    
    private bool IsHost()
    {
        if (_multiplayer != null && _multiplayer.IsConnected)
        {
            return _multiplayer.Me.Index == 0; 
        }
        return true; 
    }

    public void LeaveBehind()
    {
        if (!IsHost()) return;
        resourceManager.LoseManPower(leaveBehind_ManPowerCost);
        resourceManager.LoseMoney(leaveBehind_MoneyCost);
    }

    public void Digitalize()
    {
        if (!IsHost()) return;
        resourceManager.LoseManPower(digitalize_ManPowerCost);
        resourceManager.LoseMoney(digitalize_MoneyCost);
    }

    public void InvestRepair()
    {
        if (!IsHost()) return;
        resourceManager.LoseManPower(investRepair_ManPowerCost);
        resourceManager.LoseMoney(investRepair_MoneyCost);
    }

    public void RelocateBig()
    {
        if (!IsHost()) return;
        resourceManager.LoseManPower(relocateBig_ManPowerCost);
        resourceManager.LoseMoney(relocateBig_MoneyCost);
    }

    public void RelocateSmall()
    {
        if (!IsHost()) return;
        resourceManager.LoseManPower(relocateSmall_ManPowerCost);
        resourceManager.LoseMoney(relocateSmall_MoneyCost);
    }
}