using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayManager : MonoBehaviour
{
    public static InfoDisplayManager Instance;

    [SerializeField] private List<GameObject> infoObjects = new List<GameObject>();

    [Header("Canvas 1")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text peopleScoreText;
    [SerializeField] private TMP_Text importanceText;
    [SerializeField] private TMP_Text moneyCostText;
    [SerializeField] private TMP_Text manPowerCostText;
    [SerializeField] private Image infoImageDisplay;

    [Header("Canvas 2")]
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
            if (obj != null && obj.activeSelf)
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

        // -------- Canvas 1 --------
        if (titleText != null) titleText.text = activeInfo.Title;
        if (infoText != null) infoText.text = activeInfo.Information;
        if (peopleScoreText != null) peopleScoreText.text = $"People Score: {activeInfo.PeopleScore}";
        if (importanceText != null) importanceText.text = $"Importance: {activeInfo.Importance}";
        if (moneyCostText != null) moneyCostText.text = $"Money Cost: {activeInfo.MoneyCost}";
        if (manPowerCostText != null) manPowerCostText.text = $"ManPower Cost: {activeInfo.ManPowerCost}";

        if (infoImageDisplay != null)
        {
            if (activeInfo.InfoImage != null)
            {
                infoImageDisplay.sprite = activeInfo.InfoImage;
                infoImageDisplay.color = Color.white;
                infoImageDisplay.preserveAspect = false; 
                infoImageDisplay.gameObject.SetActive(true);

                RectTransform rt = infoImageDisplay.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(activeInfo.ImageWidth, activeInfo.ImageHeight);
            }
            else
            {
                infoImageDisplay.gameObject.SetActive(false);
            }
        }

        // -------- Canvas 2 --------
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
                infoImageDisplay2.color = Color.white;
                infoImageDisplay2.preserveAspect = false;
                infoImageDisplay2.gameObject.SetActive(true);

                RectTransform rt2 = infoImageDisplay2.GetComponent<RectTransform>();
                rt2.sizeDelta = new Vector2(activeInfo.ImageWidth2, activeInfo.ImageHeight2);
            }
            else
            {
                infoImageDisplay2.gameObject.SetActive(false);
            }
        }


    }
}
