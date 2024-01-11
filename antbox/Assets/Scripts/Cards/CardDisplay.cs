using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    private System.Random random = new System.Random();
    public Card card;
    public Image image;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public GameObject infoCanvas;
    public GameObject activityMenu;
    public bool canBeSelected=true;
    void Start()
    {
        cardName.text=card.name;
        cardDescription.text=card.description;
        image.sprite=card.artWorks;
        HideInfo();
        
    }
    public Activity[] GenerateActivitiesPerComplexity(bool isBoss){
        //TODO:Añadir complejidad dependiendo de los efectos de la carta
        Activity[] allActivities=Resources.LoadAll<Activity>("Activities");
        int n=1;
        if(isBoss) n=10;
        Activity[] activities=new Activity[n];
        for(int i=0;i<n;i++){
            int v=random.Next(0,allActivities.Length);
            activities[i]=allActivities[v];
        }
        return activities;

    }

    public void UseCard(){
        ActivityMenu activityMenu=FindObjectOfType<ActivityMenu>(true);
        activityMenu.SetActivitiesAndStartPlaying(GenerateActivitiesPerComplexity(false),false);
        ContainerData containerData=FindObjectOfType<ContainerData>();
        containerData.executableActions=card.actions;
        containerData.RemoveCardFromHand(this);
    }
    public void ShowCardData(){
        if(canBeSelected){
            UIManager[] uIManagers=FindObjectsOfType<UIManager>(true);
        foreach(UIManager ui in uIManagers){
            ui.HideInfo();
        }
        UIFarmManager[] uIFarmManagers=FindObjectsOfType<UIFarmManager>(true);
        foreach(UIFarmManager ui in uIFarmManagers){
            ui.HideInfo();
        }
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
        if(anyItem!=null){
            anyItem.MakeEveryoneUnselectableAndUnselected();
        }
        ContainerData containerData=FindObjectOfType<ContainerData>();
        foreach(CardDisplay card in containerData.cardsInHand){
            card.infoCanvas.gameObject.SetActive(false);
        }
        infoCanvas.gameObject.SetActive(true);
        }
    }
    public void HideInfo()
    {
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
        if(anyItem!=null){
            anyItem.MakeEveryoneSelectable();
        }
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
    public void MakeEveryCardUnselectable(){
        foreach(CardDisplay cardDisplay in FindObjectsOfType<CardDisplay>()){
            cardDisplay.canBeSelected=false;
        }
    }
    public void MakeEveryCardSelectable(){
        foreach(CardDisplay cardDisplay in FindObjectsOfType<CardDisplay>()){
            cardDisplay.canBeSelected=true;
        }
    }

}
