using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum AvailableActions{
    EAT,DRINK,SLEEP,GROW,DIG,MOVE,CANCEL_ACTION
}
public class UIManager : MonoBehaviour
{
    public GameObject infoCanvas; // Referencia al objeto Canvas de tu UI.
    public TextMeshProUGUI hpText; // Referencia al objeto Text para mostrar el nombre.
    public TextMeshProUGUI hungerText; // Referencia al objeto Text para mostrar la descripción.
    public TextMeshProUGUI thirstText; // Referencia al objeto Text para mostrar las estadísticas.

    public bool isQueen=false;

    public TextMeshProUGUI energyText;

    public TextMeshProUGUI ageText;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI farminSpeedText;

    public GameObject eatButton;
    public GameObject drinkButton;
    public GameObject sleepButton;
    public GameObject cancelButton;
    public GameObject moveButton;
    public GameObject farmingButton;
    //falta el botón de dig

    
    public List<AvailableActions> availableActionsWhenIsDiggingOrFarming=new List<AvailableActions>{
        AvailableActions.DRINK,
        AvailableActions.EAT,
        AvailableActions.CANCEL_ACTION};
    
    public List<AvailableActions> availableActionsWhenIsDoingNothing=new List<AvailableActions>{
        AvailableActions.DRINK,
        AvailableActions.EAT,
        AvailableActions.SLEEP,
        AvailableActions.GROW,
        AvailableActions.DIG,
        AvailableActions.MOVE};
    
    public List<AvailableActions> availableActionsWhenItsSleeping=new List<AvailableActions>{
        AvailableActions.CANCEL_ACTION};


    void Update(){
        if(!isQueen){
            AntStats antStats=this.gameObject.GetComponentInParent<AntStats>();
            UpdateCanvasWithAntStats(antStats,this.transform.parent.name);
            ShowAvailableButtonsForAnt(antStats);
        }else{
            QueenStats queenStats=this.gameObject.GetComponentInParent<QueenStats>();
            UpdateCanvasWithQueenStats(queenStats,this.transform.parent.name);
        }
    }

    void ShowAvailableButtonsForAnt(AntStats stats){
        if(stats.GetAction().Equals(ActualAction.FARMING) || stats.GetAction().Equals(ActualAction.DIGGING)){
            ProcessAvailableActions(availableActionsWhenIsDiggingOrFarming,stats);
        }else if(stats.GetAction().Equals(ActualAction.NOTHING)){
            ProcessAvailableActions(availableActionsWhenIsDoingNothing,stats);
        }else{
            ProcessAvailableActions(availableActionsWhenItsSleeping,stats);
        }
    }


    void ProcessAvailableActions(List<AvailableActions> availableActions,AntStats stats){
        List<GameObject> allButtons=new List<GameObject>{farmingButton,eatButton,drinkButton,sleepButton,moveButton,cancelButton};
        foreach(AvailableActions availableAction in availableActions){
            if(availableAction.Equals(AvailableActions.GROW)){
                allButtons.Remove(farmingButton);
                farmingButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.MOVE)){
                allButtons.Remove(moveButton);
                moveButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.DIG)){
                //TODO: Añadir lineas cuando exista el botón de excavar
            }else if(availableAction.Equals(AvailableActions.SLEEP)){
                //TODO: Añadir lineas cuando exista el botón de dormir
            }else if(availableAction.Equals(AvailableActions.EAT)){
                allButtons.Remove(eatButton);
                eatButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.DRINK)){
                allButtons.Remove(drinkButton);
                drinkButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.CANCEL_ACTION)){
                allButtons.Remove(cancelButton);
                cancelButton.SetActive(true);
            }
        }
        foreach(GameObject notAvailableButton in allButtons){
            notAvailableButton.SetActive(false);
        }
    }




    public void ShowInfo(){
        infoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo(string hp, string hunger, string thirst,string age,string name,string energy)
    {
        UpdateInfo(hp,hunger,thirst,age,name,energy);
        ShowInfo();
    }

    public void UpdateInfo(string hp, string hunger, string thirst,string age,string name,string energy){
        hpText.text = "HP:" + hp;
        hungerText.text = "Hunger:" + hunger;
        thirstText.text = "Thirst:" + thirst;
        ageText.text="Age:"+age;
        nameText.text="Name:"+name;
        if(!isQueen) energyText.text="Energy:"+energy;
    }

    public void UpdateCanvasWithAntStats(AntStats antStats,string name){
        hpText.text="HP:"+antStats.GetTextHP();
        hungerText.text = "Hunger:"+antStats.GetTextHunger();
        thirstText.text ="Thirst:" +antStats.GetTextThirst();
        ageText.text="Age:"+antStats.GetTextAge();
        energyText.text="Energy:"+antStats.GetEnergyText();
        nameText.text="Name:"+name;
        farminSpeedText.text="Farming Speed:"+antStats.GetFarminSpeed();
    }
    public void UpdateCanvasWithQueenStats(QueenStats queenStats,string name){
        hpText.text="HP:"+queenStats.GetTextHP();
        hungerText.text = "Hunger:"+queenStats.GetTextHunger();
        thirstText.text ="Thirst:" +queenStats.GetTextThirst();
        ageText.text="Age:"+queenStats.GetTextAge();
        nameText.text="Name:"+name;
        isQueen=true;
    }

    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
