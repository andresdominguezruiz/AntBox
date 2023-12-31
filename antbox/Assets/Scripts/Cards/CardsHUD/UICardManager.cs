using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardManager : MonoBehaviour
{
    public GameObject infoCanvas;
    public Image image;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public Button useButton;
    public Button cancelButton;

    void Start(){
        infoCanvas=this.gameObject;
        CardDisplay cardDisplay=this.gameObject.GetComponentInParent<CardDisplay>();
        image.sprite=cardDisplay.card.artWorks;
        cardName.text=cardDisplay.card.name;
        cardDescription.text=cardDisplay.card.description;
        Button[] allButtons=infoCanvas.GetComponentsInChildren<Button>();
        foreach(Button b in allButtons){
            if(b.gameObject.CompareTag("UseButton")) useButton=b;
            else if(b.gameObject.CompareTag("CancelButton")) cancelButton=b;
        }
    }
    public void Init(){
        infoCanvas=this.gameObject;
        CardDisplay cardDisplay=this.gameObject.GetComponentInParent<CardDisplay>();
        image.sprite=cardDisplay.card.artWorks;
        cardName.text=cardDisplay.card.name;
        cardDescription.text=cardDisplay.card.description;
        Button[] allButtons=infoCanvas.GetComponentsInChildren<Button>();
        foreach(Button b in allButtons){
            if(b.gameObject.CompareTag("UseButton")) useButton=b;
            else if(b.gameObject.CompareTag("CancelButton")) cancelButton=b;
        }
    }
}
