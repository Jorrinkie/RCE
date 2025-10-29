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

        titleText.text = activeInfo.Title;
        infoText.text = activeInfo.Information;
        peopleScoreText.text = $"People Score: {activeInfo.PeopleScore}";
        importanceText.text = $"Importance: {activeInfo.Importance}";
        moneyCostText.text = $"Money Cost: {activeInfo.MoneyCost}";
        manPowerCostText.text = $"ManPower Cost: {activeInfo.ManPowerCost}";

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
    }
}

