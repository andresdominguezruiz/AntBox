using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public enum AvailableActions{
    EAT,DRINK,SLEEP,GROW,DIG,MOVE,CANCEL_ACTION,CHANGE_DIRECTIONS,INIT_ATTACK,CHANGE_TARGET,HELP
}
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject infoCanvas; // Referencia al objeto Canvas de tu UI.
    [SerializeField]
    private TextMeshProUGUI hpText; // Referencia al objeto Text para mostrar el nombre.
    [SerializeField]
    private TextMeshProUGUI hungerText; // Referencia al objeto Text para mostrar la descripción.
    [SerializeField]
    private TextMeshProUGUI thirstText; // Referencia al objeto Text para mostrar las estadísticas.
    [SerializeField]
    private TextMeshProUGUI poisonText; // Referencia al objeto Text para mostrar las estadísticas.

    [SerializeField]
    private bool isQueen = false;

    [SerializeField]
    private TextMeshProUGUI energyText;

    [SerializeField]
    private TextMeshProUGUI ageText;

    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI farminSpeedText;
    [SerializeField]
    private TextMeshProUGUI diggingSpeedText;
    [SerializeField]
    private TextMeshProUGUI recoverSpeedText;
    [SerializeField]
    private TextMeshProUGUI damageText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedText;


    [SerializeField]
    private GameObject eatButton;
    
    [SerializeField]
    private GameObject drinkButton;
    
    [SerializeField]
    private GameObject sleepButton;
    
    [SerializeField]
    private GameObject cancelButton;
    
    [SerializeField]
    private GameObject moveButton;
    
    [SerializeField]
    private GameObject farmingButton;
    
    [SerializeField]
    private GameObject initAttackButton;
    
    [SerializeField]
    private GameObject upButton;
    
    [SerializeField]
    private GameObject downButton;
    
    [SerializeField]
    private GameObject rightButton;
    
    [SerializeField]
    private GameObject leftButton;

    [SerializeField]
    private GameObject digButton;
    
    [SerializeField]
    private GameObject helpButton;


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
        AvailableActions.MOVE,
        AvailableActions.INIT_ATTACK};
    
    public List<AvailableActions> availableActionsWhenItsSleeping=new List<AvailableActions>{
        AvailableActions.CANCEL_ACTION};
    
    public List<AvailableActions> availableActionsWhenItsFighting=new List<AvailableActions>{
        AvailableActions.CANCEL_ACTION,
        AvailableActions.CHANGE_TARGET,
        AvailableActions.HELP
    };

    public GameObject InfoCanvas { get => infoCanvas; set => infoCanvas = value; }
    public TextMeshProUGUI HpText { get => hpText; set => hpText = value; }
    public TextMeshProUGUI HungerText { get => hungerText; set => hungerText = value; }
    public TextMeshProUGUI ThirstText { get => thirstText; set => thirstText = value; }
    public TextMeshProUGUI PoisonText { get => poisonText; set => poisonText = value; }
    public bool IsQueen { get => isQueen; set => isQueen = value; }
    public TextMeshProUGUI EnergyText { get => energyText; set => energyText = value; }
    public TextMeshProUGUI AgeText { get => ageText; set => ageText = value; }
    public TextMeshProUGUI NameText { get => nameText; set => nameText = value; }
    public TextMeshProUGUI FarminSpeedText { get => farminSpeedText; set => farminSpeedText = value; }
    public TextMeshProUGUI DiggingSpeedText { get => diggingSpeedText; set => diggingSpeedText = value; }
    public TextMeshProUGUI RecoverSpeedText { get => recoverSpeedText; set => recoverSpeedText = value; }
    public TextMeshProUGUI DamageText { get => damageText; set => damageText = value; }
    public TextMeshProUGUI AttackSpeedText { get => attackSpeedText; set => attackSpeedText = value; }
    public GameObject EatButton { get => eatButton; set => eatButton = value; }
    public GameObject DrinkButton { get => drinkButton; set => drinkButton = value; }
    public GameObject SleepButton { get => sleepButton; set => sleepButton = value; }
    public GameObject CancelButton { get => cancelButton; set => cancelButton = value; }
    public GameObject MoveButton { get => moveButton; set => moveButton = value; }
    public GameObject FarmingButton { get => farmingButton; set => farmingButton = value; }
    public GameObject InitAttackButton { get => initAttackButton; set => initAttackButton = value; }
    public GameObject UpButton { get => upButton; set => upButton = value; }
    public GameObject DownButton { get => downButton; set => downButton = value; }
    public GameObject RightButton { get => rightButton; set => rightButton = value; }
    public GameObject LeftButton { get => leftButton; set => leftButton = value; }
    public GameObject DigButton { get => digButton; set => digButton = value; }
    public GameObject HelpButton { get => helpButton; set => helpButton = value; }

    public void CancelAntAction(){
        AntStats stats=this.gameObject.GetComponentInParent<AntStats>();
        if(stats.GetAction().Equals(ActualAction.FARMING)){
            CancelFarming(stats);
        }
        else if(stats.GetAction().Equals(ActualAction.DIGGING)){
            CancelDigging(stats);
        }
        else if(stats.GetAction().Equals(ActualAction.SLEEPING)){
            CancelSleeping(stats);
        }
        else if(stats.GetAction().Equals(ActualAction.ATTACKING)){
            stats.StopAttacking();
        }
    }

    public void CancelSleeping(AntStats stats){
        stats.DoNothing();

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

    public void StartToSleep(){
        AntStats stats=this.gameObject.GetComponentInParent<AntStats>();
        stats.GoToSleep();

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
                break;
            }
        }
    }


    void Update(){
        if(!IsQueen){
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
        }else if(stats.GetAction().Equals(ActualAction.ATTACKING)){
            ProcessAvailableActions(availableActionsWhenItsFighting,stats);
        }else{
            ProcessAvailableActions(availableActionsWhenItsSleeping,stats);
        }
    }



    void ProcessAvailableActions(List<AvailableActions> availableActions,AntStats stats){
        List<GameObject> allButtons=new List<GameObject>{FarmingButton,EatButton,DrinkButton
        ,SleepButton,MoveButton,CancelButton,DigButton,UpButton,RightButton,LeftButton
        ,DownButton,InitAttackButton,HelpButton};
        ExcavationMovement ex=this.gameObject.GetComponentInParent<ExcavationMovement>();
        FarmStats farm=FindFirstObjectByType<FarmStats>();
        foreach(AvailableActions availableAction in availableActions){
            if(availableAction.Equals(AvailableActions.GROW) && farm!=null && stats.GetActualEnergy()>=farm.energyCostOfCycle){
                allButtons.Remove(FarmingButton);
                FarmingButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.MOVE)){
                allButtons.Remove(MoveButton);
                MoveButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.INIT_ATTACK)){
                allButtons.Remove(InitAttackButton);
                InitAttackButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.DIG) && ex.CanDig()){
                allButtons.Remove(DigButton);
                DigButton.SetActive(true);
            }else if( availableAction.Equals(AvailableActions.CHANGE_DIRECTIONS)&& ex.IsDigging){
                    if(ex.CanGoLeft()){
                        allButtons.Remove(LeftButton);
                        LeftButton.SetActive(true);
                    }
                    if(ex.CanGoRight()){
                        allButtons.Remove(RightButton);
                        RightButton.SetActive(true);
                    }
                    if(ex.CanGoDown()){
                        allButtons.Remove(DownButton);
                        DownButton.SetActive(true);
                    }
                    if(ex.CanGoUp()){
                        allButtons.Remove(UpButton);
                        UpButton.SetActive(true);
                    }
            }else if(availableAction.Equals(AvailableActions.SLEEP)){
                allButtons.Remove(SleepButton);
                SleepButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.EAT)){
                allButtons.Remove(EatButton);
                EatButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.DRINK)){
                allButtons.Remove(DrinkButton);
                DrinkButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.CANCEL_ACTION)){
                allButtons.Remove(CancelButton);
                CancelButton.SetActive(true);
            }else if(availableAction.Equals(AvailableActions.HELP)){
                allButtons.Remove(HelpButton);
                HelpButton.SetActive(true);
            }
        }
        foreach(GameObject notAvailableButton in allButtons){
            notAvailableButton.SetActive(false);
        }
    }




    public void ShowInfo(){
        InfoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo(string hp, string hunger, string thirst,string age,string name,string energy)
    {
        UpdateInfo(hp,hunger,thirst,age,name,energy);
        ShowInfo();
    }

    public void UpdateInfo(string hp, string hunger, string thirst,string age,string name,string energy){
        HpText.text = "HP:" + hp;
        HungerText.text = "Hunger:" + hunger;
        ThirstText.text = "Thirst:" + thirst;
        AgeText.text="Age:"+age;
        NameText.text="Name:"+name;
        if(!IsQueen){
            EnergyText.text="Energy:"+energy;
        }
    }


    public void UpdateCanvasWithAntStats(AntStats antStats,string name){
        HpText.text="HP:"+antStats.GetTextHP();
        HungerText.text = "Hunger:"+antStats.GetTextHunger();
        ThirstText.text ="Thirst:" +antStats.GetTextThirst();
        AgeText.text="Age:"+antStats.GetTextAge();
        EnergyText.text="Energy:"+antStats.GetEnergyText();
        NameText.text="Name:"+name;
        FarminSpeedText.text="Farming Speed:"+antStats.GetFarmingSpeed();
        DiggingSpeedText.text="Digging Speed:"+antStats.GetDiggingSpeed();
        RecoverSpeedText.text="Recover Speed:"+antStats.GetRecoverSpeed();
        if(antStats.PoisonSecons<=0){
             PoisonText.text="";
        }
        else{
            PoisonText.text="Poison: "+antStats.PoisonSecons;
        }
        DamageText.text="Damage:"+antStats.battleStats.Damage;
        AttackSpeedText.text="Attack Speed:"+antStats.battleStats.AttackSpeed+" .s";
    }
    public void UpdateCanvasWithQueenStats(QueenStats queenStats,string name){
        HpText.text="HP:"+queenStats.GetTextHP();
        HungerText.text = "Hunger:"+queenStats.GetTextHunger();
        ThirstText.text ="Thirst:" +queenStats.GetTextThirst();
        AgeText.text="Age:"+queenStats.GetTextAge();
        NameText.text="Name:"+name;
        IsQueen=true;
        if(queenStats.PoisonSecons<=0){
            PoisonText.text="";
        }
        else{
            PoisonText.text="Poison: "+queenStats.PoisonSecons;
        }
    }

    public void HideInfo()
    {
        InfoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
