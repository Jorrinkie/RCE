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

    public string Title => title;
    public string Information => information;
    public int PeopleScore => peopleScore;
    public int Importance => importance;
    public int MoneyCost => moneyCost;
    public int ManPowerCost => manPowerCost;
    public Sprite InfoImage => infoImage; 
}
