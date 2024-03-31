using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardManager : MonoBehaviour
{
    [SerializeField]
    private GameObject infoCanvas;

    [SerializeField]
    private Image image;
    
    [SerializeField]
    private TextMeshProUGUI cardName;

    [SerializeField]
    private TextMeshProUGUI cardDescription;
    
    [SerializeField]
    private Button useButton;

    [SerializeField]
    private Button cancelButton;

    [SerializeField]
    private Button discardCardButton;

    public GameObject InfoCanvas { get => infoCanvas; set => infoCanvas = value; }
    public Image Image { get => image; set => image = value; }
    public TextMeshProUGUI CardName { get => cardName; set => cardName = value; }
    public TextMeshProUGUI CardDescription { get => cardDescription; set => cardDescription = value; }
    public Button UseButton { get => useButton; set => useButton = value; }
    public Button CancelButton { get => cancelButton; set => cancelButton = value; }
    public Button DiscardCardButton { get => discardCardButton; set => discardCardButton = value; }

    void Start(){
        Init();
    }
    public void Init(){
        InfoCanvas=this.gameObject;
        CardDisplay cardDisplay=this.gameObject.GetComponentInParent<CardDisplay>();
        Image.sprite=cardDisplay.card.artWorks;
        CardName.text=cardDisplay.card.name;
        CardDescription.text=cardDisplay.card.description;
        Button[] allButtons=InfoCanvas.GetComponentsInChildren<Button>();
        foreach(Button b in allButtons){
            if(b.gameObject.CompareTag("UseButton")){
                UseButton=b;
            }
            else if(b.gameObject.CompareTag("CancelButton")){
                CancelButton=b;
            }
            else if(b.gameObject.CompareTag("DiscardButton")){
                DiscardCardButton=b;
            }
        }
    }
}
