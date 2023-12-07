using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public enum AvailableActions{
    EAT,DRINK,SLEEP,GROW,DIG,MOVE,CANCEL_ACTION,CHANGE_DIRECTIONS
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
    public TextMeshProUGUI diggingSpeedText;

    public GameObject eatButton;
    public GameObject drinkButton;
    public GameObject sleepButton;
    public GameObject cancelButton;
    public GameObject moveButton;
    public GameObject farmingButton;
    public GameObject upButton;
    public GameObject downButton;
    public GameObject rightButton;
    public GameObject leftButton;
    
    public GameObject digButton;

    
    public List<AvailableActions> availableActionsWhenIsFarming=new List<AvailableActions>{
        AvailableActions.DRINK,
        AvailableActions.EAT,
        AvailableActions.CANCEL_ACTION};
    
    public List<AvailableActions> availableActionsWhenIsDigging=new List<AvailableActions>{
        AvailableActions.DRINK,
        AvailableActions.EAT,
        AvailableActions.CANCEL_ACTION,
        AvailableActions.CHANGE_DIRECTIONS
    };
    
    public List<AvailableActions> availableActionsWhenIsDoingNothing=new List<AvailableActions>{
        AvailableActions.DRINK,
        AvailableActions.EAT,
        AvailableActions.SLEEP,
        AvailableActions.GROW,
        AvailableActions.DIG,
        AvailableActions.MOVE};
    
    public List<AvailableActions> availableActionsWhenItsSleeping=new List<AvailableActions>{
        AvailableActions.CANCEL_ACTION};

    
    public void CancelAntAction(){
        AntStats stats=this.gameObject.GetComponentInParent<AntStats>();
        if(stats.GetAction().Equals(ActualAction.FARMING)) CancelFarming(stats);
        else if(stats.GetAction().Equals(ActualAction.DIGGING)) CancelDigging(stats);
    }

    public void GoLeft(){
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        ex.Left();
    }
    public void GoRight(){
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        ex.Right();
    }
    public void GoUp(){
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        ex.Up();
    }
    public void GoDown(){
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        ex.Down();
    }

    public void CancelDigging(AntStats antStats){
        antStats.gameObject.GetComponent<NavMeshAgent>().enabled=true;
        antStats.gameObject.GetComponent<ExcavationMovement>().StopDigging();
        antStats.DoNothing();
    }

    
    public void CancelFarming(AntStats stats){
        FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
        foreach(FarmStats farm in allFarms){
            if(farm.antsOfFarm.Contains(stats.gameObject)){
                farm.antsOfFarm.Remove(stats.gameObject);
                farm.antsWorkingInFarm.Remove(stats.gameObject);
                stats.DoNothing();
                stats.gameObject.GetComponent<NavMeshAgent>().isStopped=false;
                stats.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.transform.position);
                Debug.Log("He cancelado, ahora su estado es "+stats.GetAction());
                break;
            }
        }
    }


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
        if(stats.GetAction().Equals(ActualAction.FARMING)){
            ProcessAvailableActions(availableActionsWhenIsFarming,stats);
        }else if(stats.GetAction().Equals(ActualAction.NOTHING)){
            ProcessAvailableActions(availableActionsWhenIsDoingNothing,stats);
        }else if(stats.GetAction().Equals(ActualAction.DIGGING)){
            ProcessAvailableActions(availableActionsWhenIsDigging,stats);
        }else{
            ProcessAvailableActions(availableActionsWhenItsSleeping,stats);
        }
    }



    void ProcessAvailableActions(List<AvailableActions> availableActions,AntStats stats){
        List<GameObject> allButtons=new List<GameObject>{farmingButton,eatButton,drinkButton
        ,sleepButton,moveButton,cancelButton,digButton,upButton,rightButton,leftButton,downButton};
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        foreach(AvailableActions availableAction in availableActions){
            if(availableAction.Equals(AvailableActions.GROW)){
                allButtons.Remove(farmingButton);
                farmingButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.MOVE)){
                allButtons.Remove(moveButton);
                moveButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.DIG)){
                allButtons.Remove(digButton);
                digButton.SetActive(true);
            }else if( availableAction.Equals(AvailableActions.CHANGE_DIRECTIONS)&& ex.IsDigging()){
                    if(ex.CanGoLeft()){
                        allButtons.Remove(leftButton);
                        leftButton.SetActive(true);
                    }
                    if(ex.CanGoRight()){
                        allButtons.Remove(rightButton);
                        leftButton.SetActive(true);
                    }
                    if(ex.CanGoDown()){
                        allButtons.Remove(downButton);
                        downButton.SetActive(true);
                    }
                    if(ex.CanGoUp()){
                        allButtons.Remove(upButton);
                        upButton.SetActive(true);
                    }
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
        diggingSpeedText.text="Digging Speed:"+antStats.GetDiggingSpeed();
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
