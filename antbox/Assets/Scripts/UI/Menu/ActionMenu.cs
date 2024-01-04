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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChoosing && index>actions.Count-1){
            FinishActionMenu();
        }
        
    }
    void FinishActionMenu(){
        this.actions=null;
        index=0;
        this.gameObject.SetActive(false);
    }

    public void InitVictoryActions(List<Action> actions){
        this.actions=actions;
        index=0;
        this.gameObject.SetActive(true);
        ProcessActualAction();
    }

//1 NIVEL
    void ProcessActualAction(){
        Action actualAction=actions[index];
        if(actualAction.type.Equals(ActionType.UPDATE)) ApplyUpdateAction(actualAction);
        else if(actualAction.type.Equals(ActionType.ADD)) ApplyAddAction(actualAction);

        index++;
        if(!(index>actions.Count-1)) ProcessActualAction();
    }
    //2 NIVEL
    void ApplyUpdateAction(Action actualAction){
        if(actualAction.NoNeedToChooseItemToApplyUpdateAction()){
            for(int i=0;i<actualAction.uses;i++){
                if(actualAction.interactionType.Equals(InteractionType.ANY)) UpdateAny(actualAction);
                else if(actualAction.interactionType.Equals(InteractionType.ALL)) UpdateAll(actualAction);
            }
        }
    }
    void UpdateAll(Action actualAction){
        if(actualAction.destination.Equals(Destination.ANT)){
            AntStats[] antStats=FindObjectsOfType<AntStats>(false);
            foreach(AntStats ant in antStats){
                ant.ProcessUpdateEffectOfAction(actualAction);
            }
        }
        else if(actualAction.destination.Equals(Destination.FARM)){
            FarmStats[] farmStats=FindObjectsOfType<FarmStats>(false);
            foreach(FarmStats farm in farmStats){
                farm.ProcessUpdateEffectOfAction(actualAction);
            }
        }else if(actualAction.destination.Equals(Destination.ANTHILL)){
            CharacterStats[] characterStats=FindObjectsOfType<CharacterStats>();
            foreach(CharacterStats character in characterStats){
                character.ProcessUpdateEffectOfAction(actualAction);
            }
        }else if(actualAction.destination.Equals(Destination.QUEEN)){
            QueenStats[] queenStats=FindObjectsOfType<QueenStats>(false);
            foreach(QueenStats queen in queenStats){
                queen.ProcessUpdateEffectOfAction(actualAction);
            }
        }else if(actualAction.destination.Equals(Destination.CONTAINER)){
            ContainerData container=FindObjectOfType<ContainerData>(false);
            if(container!=null) container.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.FOOD_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.FOOD)){
                    farm.ProcessUpdateEffectOfAction(actualAction);
                }
            }
        }else if(actualAction.destination.Equals(Destination.WATER_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.WATER)){
                    farm.ProcessUpdateEffectOfAction(actualAction);
                }
            }
        }
    }
    //3 NIVEL
    void UpdateAny(Action actualAction){
        if(actualAction.destination.Equals(Destination.ANT)){
            AntStats antStats=FindObjectOfType<AntStats>(false);
            if(antStats!=null) antStats.ProcessUpdateEffectOfAction(actualAction);
        }
        else if(actualAction.destination.Equals(Destination.FARM)){
            FarmStats farmStats=FindObjectOfType<FarmStats>(false);
            if(farmStats!=null) farmStats.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.ANTHILL)){
            CharacterStats characterStats=FindObjectOfType<CharacterStats>();
            if(characterStats!=null) characterStats.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.QUEEN)){
            QueenStats queenStats=FindObjectOfType<QueenStats>(false);
            if(queenStats!=null) queenStats.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.CONTAINER)){
            ContainerData container=FindObjectOfType<ContainerData>(false);
            if(container!=null) container.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.FOOD_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            FarmStats firstFoodFarm=null;
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.FOOD)){
                    firstFoodFarm=farm;
                    break;
                }
            }
            if(firstFoodFarm!=null) firstFoodFarm.ProcessUpdateEffectOfAction(actualAction);
        }else if(actualAction.destination.Equals(Destination.WATER_FARM)){
            FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
            FarmStats firstWaterFarm=null;
            foreach(FarmStats farm in allFarms){
                if(farm.GetTypeOfFarm().Equals(Type.WATER)){
                    firstWaterFarm=farm;
                    break;
                }
            }
            if(firstWaterFarm!=null) firstWaterFarm.ProcessUpdateEffectOfAction(actualAction);
        }
    }


//2 NIVEL
    void ApplyAddAction(Action actualAction){
        for(int i=0;i<actualAction.uses;i++){
            if(actualAction.destination.Equals(Destination.ANT)){
                GenerationTilemap generation=FindObjectOfType<GenerationTilemap>();
                if(generation!=null) generation.AddNewAnt();
            }else if(actualAction.destination.Equals(Destination.FOOD_FARM)
             || actualAction.destination.Equals(Destination.WATER_FARM) 
             || actualAction.destination.Equals(Destination.FARM)){
                FarmGenerator farmGenerator=FindObjectOfType<FarmGenerator>();
                if(farmGenerator!=null && actualAction.destination.Equals(Destination.FOOD_FARM)){
                    farmGenerator.AddNewFarm(Type.FOOD);
                }else if(farmGenerator!=null && actualAction.destination.Equals(Destination.WATER_FARM)){
                    farmGenerator.AddNewFarm(Type.WATER);
                }else if(farmGenerator!=null && actualAction.destination.Equals(Destination.FARM)){
                    farmGenerator.AddNewFarm();
                }
            }
        }
    }
}
