using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    private List<Action> actions;
    [SerializeField] private TextMeshProUGUI consoleText;
    private int index;
    private bool isChoosing=false;
    private int actualUses=0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.isPaused){
            if(!isChoosing && index>actions.Count-1){
                FinishActionMenu();
            }
            else if(isChoosing && index<=actions.Count-1){
                bool result=MakeActionByChoosingItem();
                if(result){
                    Action actualAction=actions[index];
                    actualUses--;
                        if(actualUses<=0){
                            isChoosing=false;
                            index++;
                            Time.timeScale=1f;
                            if(index>actions.Count-1){
                                FinishActionMenu();
                            }
                            else{
                                ProcessActualAction();
                            }
                        }
                        else{
                            consoleText.text="Choose "+actualUses+" "
                            +actualAction.Destination.ToString()+" to "+actualAction.Type.ToString();
                        }
            }
        }
        }
        
    }

    bool MakeActionByChoosingItem(){
        bool res=false;
        Action actualAction=actions[index];
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if(hit.collider!=null){
                bool correctExecution=false;
                if((hit.collider.CompareTag("Ant") && 
                (actualAction.Destination.Equals(Destination.ANT) || actualAction.Destination.Equals(Destination.ANTHILL)))
                || (hit.collider.CompareTag("Queen") && (actualAction.Destination.Equals(Destination.QUEEN)
                 || actualAction.Destination.Equals(Destination.ANTHILL)))){

                    CharacterStats ant=hit.collider.gameObject.GetComponent<CharacterStats>();
                    ant.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
                    correctExecution=true;
                }else if(hit.collider.CompareTag("Farm") && (actualAction.Destination.Equals(Destination.FARM)
                 || actualAction.Destination.Equals(Destination.FOOD_FARM)
                  || actualAction.Destination.Equals(Destination.WATER_FARM))){
                    
                    FarmStats farmStats=hit.collider.gameObject.GetComponent<FarmStats>();
                    if((actualAction.Destination.Equals(Destination.FOOD_FARM) && farmStats.GetTypeOfFarm().Equals(Type.FOOD))
                    || (actualAction.Destination.Equals(Destination.WATER_FARM) && farmStats.GetTypeOfFarm().Equals(Type.WATER))
                    || actualAction.Destination.Equals(Destination.FARM)){
                        farmStats.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
                        correctExecution=true;
                    }
                  }
                res=correctExecution;
            }
        }
        return res;
    }
    void FinishActionMenu(){
        this.actions=null;
        Time.timeScale=1f;
        index=0;
        this.gameObject.SetActive(false);
        ContainerData containerData=FindObjectOfType<ContainerData>(false);
        containerData.GoBackToGameAfterActivity();
        Clock clock=FindObjectOfType<Clock>();
        if(clock!=null){
            consoleText.text=clock.messageOfEvent;
        }
    }

    public void InitActions(List<Action> actions){
        this.actions=actions;
        index=0;
        this.gameObject.SetActive(true);
        ProcessActualAction();
    }

//1 NIVEL
    void ProcessActualAction(){
        Action actualAction=actions[index];
        if(actualAction.Type.Equals(ActionType.UPDATE)){
            ApplyUpdateAction(actualAction);
        }
        else if(actualAction.Type.Equals(ActionType.ADD)){
            ApplyAddAction(actualAction);
        }
        else if(actualAction.Type.Equals(ActionType.DELETE)){
            ApplyDeleteAction(actualAction);
        }
        if(!isChoosing){
            index++;
            if(!(index>actions.Count-1)){
                ProcessActualAction();
            }
        }
    }

    //2 NIVEL
    void ApplyDeleteAction(Action actualAction){
        for(int i=0;i<actualAction.Uses;i++){
                //SOLO VA A SER DE MOMENTO BORRADOS DE HORMIGAS:
                DeleteAny(actualAction);
            }
    }
    //3 NIVEL
    void DeleteAny(Action actualAction){
        if(actualAction.Destination.Equals(Destination.ANT)){
            AntStats[] antStats=FindObjectsOfType<AntStats>(false);
            foreach(AntStats ant in antStats){
                if(!ant.IsDead()){
                    ant.Die();
                    break;
                }
            }
        }
    }
    //2 NIVEL
    void ApplyUpdateAction(Action actualAction){
        if(actualAction.NoNeedToChooseItemToApplyUpdateAction()){
            for(int i=0;i<actualAction.Uses;i++){
                if(actualAction.InteractionType.Equals(InteractionType.ANY)){
                    UpdateAny(actualAction);
                }
                else if(actualAction.InteractionType.Equals(InteractionType.ALL)){
                    UpdateAll(actualAction);
                }
            }
        }
        else{
            PrepareToUpdateByChoosingItem(actualAction);
        }
    }
    void PrepareToUpdateByChoosingItem(Action actualAction){
        isChoosing=true;
        Time.timeScale=0f;
        actualUses=actualAction.Uses;
        consoleText.text="Choose "+actualAction.Uses+" "
        +actualAction.Destination.ToString()+" to "+actualAction.Type.ToString();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>(false);
        if(anyCardDisplay!=null){
            anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        }
        SelectableItem item=FindObjectOfType<SelectableItem>(false);
        if(item!=null){
            item.MakeEveryoneUnselectableAndUnselected();
        }

    }

    //3 NIVEL
    void UpdateAll(Action actualAction){
        if(actualAction.Destination.Equals(Destination.ANT)){
            AntStats[] antStats=FindObjectsOfType<AntStats>(false);
            foreach(AntStats ant in antStats){
                ant.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }
        else if(actualAction.Destination.Equals(Destination.FARM)){
            FarmStats[] farmStats=FindObjectsOfType<FarmStats>(false);
            foreach(FarmStats farm in farmStats){
                farm.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.ANTHILL)){
            CharacterStats[] characterStats=FindObjectsOfType<CharacterStats>();
            foreach(CharacterStats character in characterStats){
                character.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.QUEEN)){
            QueenStats[] queenStats=FindObjectsOfType<QueenStats>(false);
            foreach(QueenStats queen in queenStats){
                queen.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.CONTAINER)){
            ContainerData container=FindObjectOfType<ContainerData>(false);
            if(container!=null) container.ProcessUpdateEffectOfAction(actualAction.ContainerEffects);
        }else if(actualAction.Destination.Equals(Destination.FOOD_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.FOOD)){
                    farm.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
                }
            }
        }else if(actualAction.Destination.Equals(Destination.WATER_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.WATER)){
                    farm.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
                }
            }
        }
    }
    //3 NIVEL
    void UpdateAny(Action actualAction){
        if(actualAction.Destination.Equals(Destination.ANT)){
            AntStats antStats=FindObjectOfType<AntStats>(false);
            if(antStats!=null){
                antStats.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }
        else if(actualAction.Destination.Equals(Destination.FARM)){
            FarmStats farmStats=FindObjectOfType<FarmStats>(false);
            if(farmStats!=null){
                farmStats.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.ANTHILL)){
            CharacterStats characterStats=FindObjectOfType<CharacterStats>();
            if(characterStats!=null){
                characterStats.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.QUEEN)){
            QueenStats queenStats=FindObjectOfType<QueenStats>(false);
            if(queenStats!=null){
                queenStats.ProcessUpdateEffectOfAction(actualAction.CharacterEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.CONTAINER)){
            ContainerData container=FindObjectOfType<ContainerData>(false);
            if(container!=null){
                container.ProcessUpdateEffectOfAction(actualAction.ContainerEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.FOOD_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            FarmStats firstFoodFarm=null;
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.FOOD)){
                    firstFoodFarm=farm;
                    break;
                }
            }
            if(firstFoodFarm!=null){
                firstFoodFarm.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
            }
        }else if(actualAction.Destination.Equals(Destination.WATER_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            FarmStats firstWaterFarm=null;
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.WATER)){
                    firstWaterFarm=farm;
                    break;
                }
            }
            if(firstWaterFarm!=null){
                firstWaterFarm.ProcessUpdateEffectOfAction(actualAction.FarmEffects);
            }
        }
        else if(actualAction.Destination.Equals(Destination.PLAYER)){
            //ESTO SON PASIVAS DEL JUGADOR
            Player.Instance.ProcessUpdateEffectOfAction(actualAction);
        }
    }


//2 NIVEL
    void ApplyAddAction(Action actualAction){
        for(int i=0;i<actualAction.Uses;i++){
            if(actualAction.Destination.Equals(Destination.ANT)){
                GenerationTilemap generation=FindObjectOfType<GenerationTilemap>();
                if(generation!=null){
                    generation.AddNewAnt();
                }
            }else if(actualAction.Destination.Equals(Destination.FOOD_FARM)
             || actualAction.Destination.Equals(Destination.WATER_FARM) 
             || actualAction.Destination.Equals(Destination.FARM)){
                FarmGenerator farmGenerator=FindObjectOfType<FarmGenerator>();
                if(farmGenerator!=null && actualAction.Destination.Equals(Destination.FOOD_FARM)){
                    farmGenerator.AddNewFarmRandomly(Type.FOOD);
                }else if(farmGenerator!=null && actualAction.Destination.Equals(Destination.WATER_FARM)){
                    farmGenerator.AddNewFarmRandomly(Type.WATER);
                }else if(farmGenerator!=null && actualAction.Destination.Equals(Destination.FARM)){
                    farmGenerator.AddNewFarmRandomly();
                }
            }
        }
    }
}
