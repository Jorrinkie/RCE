using UnityEngine;

public class InfoObject : MonoBehaviour
{
    [SerializeField] private string title;
    [SerializeField, TextArea] private string information;
    [SerializeField] private int peopleScore;
    [SerializeField] private int importance;
    [SerializeField] private int moneyCost;
    [SerializeField] private int manPowerCost;

    [Header("Optional Image")]
    [SerializeField] private Sprite infoImage;

    [Header("Optional Image Size for Canvas 1")]
    [SerializeField] private float imageWidth = 100f;
    [SerializeField] private float imageHeight = 100f;

    [Header("Optional Image Size for Canvas 2")]
    [SerializeField] private float imageWidth2 = 100f;
    [SerializeField] private float imageHeight2 = 100f;

    public string Title => title;
    public string Information => information;
    public int PeopleScore => peopleScore;
    public int Importance => importance;
    public int MoneyCost => moneyCost;
    public int ManPowerCost => manPowerCost;
    public Sprite InfoImage => infoImage;

    public float ImageWidth => imageWidth;
    public float ImageHeight => imageHeight;

    public float ImageWidth2 => imageWidth2;
    public float ImageHeight2 => imageHeight2;
}