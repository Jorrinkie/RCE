using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayManager : MonoBehaviour
{
    public static InfoDisplayManager Instance;

    [SerializeField] private List<GameObject> infoObjects = new List<GameObject>();

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text peopleScoreText;
    [SerializeField] private TMP_Text importanceText;
    [SerializeField] private TMP_Text moneyCostText;
    [SerializeField] private TMP_Text manPowerCostText;
    [SerializeField] private Image infoImageDisplay;


    [SerializeField] private TMP_Text titleText2;
    [SerializeField] private TMP_Text infoText2;
    [SerializeField] private TMP_Text peopleScoreText2;
    [SerializeField] private TMP_Text importanceText2;
    [SerializeField] private TMP_Text moneyCostText2;
    [SerializeField] private TMP_Text manPowerCostText2;
    [SerializeField] private Image infoImageDisplay2;

    private InfoObject activeInfo;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        foreach (var obj in infoObjects)
        {
            if (obj.activeSelf)
            {
                var info = obj.GetComponent<InfoObject>();
                if (info != null && info != activeInfo)
                {
                    activeInfo = info;
                    UpdateUI();
                    break;
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (activeInfo == null) return;

        // --- Canvas 1 ---
        titleText.text = activeInfo.Title;
        infoText.text = activeInfo.Information;
        peopleScoreText.text = $"People Score: {activeInfo.PeopleScore}";
        importanceText.text = $"Importance: {activeInfo.Importance}";
    //    moneyCostText.text = $"Money Cost: {activeInfo.MoneyCost}";
        //manPowerCostText.text = $"ManPower Cost: {activeInfo.ManPowerCost}";

        if (infoImageDisplay != null)
        {
            if (activeInfo.InfoImage != null)
            {
                infoImageDisplay.sprite = activeInfo.InfoImage;
                infoImageDisplay.gameObject.SetActive(true);
            }
            else
            {
                infoImageDisplay.gameObject.SetActive(false);
            }
        }

        // --- Canvas 2 ---
        if (titleText2 != null) titleText2.text = activeInfo.Title;
        if (infoText2 != null) infoText2.text = activeInfo.Information;
        if (peopleScoreText2 != null) peopleScoreText2.text = $"People Score: {activeInfo.PeopleScore}";
        if (importanceText2 != null) importanceText2.text = $"Importance: {activeInfo.Importance}";
        if (moneyCostText2 != null) moneyCostText2.text = $"Money Cost: {activeInfo.MoneyCost}";
        if (manPowerCostText2 != null) manPowerCostText2.text = $"ManPower Cost: {activeInfo.ManPowerCost}";

        if (infoImageDisplay2 != null)
        {
            if (activeInfo.InfoImage != null)
            {
                infoImageDisplay2.sprite = activeInfo.InfoImage;
                infoImageDisplay2.gameObject.SetActive(true);
            }
            else
            {
                infoImageDisplay2.gameObject.SetActive(false);
            }
        }
    }
}
