using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public enum ActionType{
    ADD,UPDATE,DELETE
}
public enum InteractionType{
    ANY,ALL,CHOOSE,NONE
}
public enum Destination{
    ANT,CONTAINER,WATER_FARM,FOOD_FARM,QUEEN,FARM,ANTHILL,PLAYER,HORDE
}

public enum UpdateEffectOnPlayer{
    NONE,ALLOW_NEGATIVE_AGE,MAKE_TIME_SLOWER,DISABLE_DEAD_BY_AGE
}

public enum UpdateEffectOnAntOrQueen{
    AGE,HP_LIMIT,RESTORE_ENERGY,FEED,HYDRATE,RESTORE_HP,RESTORE_HUNGER,RESTORE_THIRST,NONE
    ,HUNGER_LIMIT,THIRST_LIMIT
    ,ENERGY_LIMIT,FARMING_SPEED,DIGGING_SPEED,RECOVER_SPEED
    //AGE,ACTUAL_HP,ACTUAL_ENERGY,ACTUAL_HUNGER,ACTUAL_THIRST
    //,ACTUAL_HUNGER,HUNGER,THIRST,HP,ENERGY,FARMING_SPEED,DIGGING_SPEED,RECOVER_SPEED
}
public enum UpdateEffectOnFarm{
    FARM_CYCLE,CAPACITY,FARM_RESOURCES,ENERGY_COST,NONE
    //RESOURCES,FARM_CYCLE,CAPACITY,NONE
}
public enum UpdateEffectOnContainer{
    FOOD,WATER,FOOD_VALUE,WATER_VALUE,MIRROR,NONE
    //FOOD,WATER,EVERYTHING,NONE
}

[System.Serializable] //Al colocar esto, permitimos que Unity pueda interpretar esta clase y que pueda construirse
//esta clase desde su interfaz
public class Action
{
    [SerializeField]
    private int uses=1;

    [SerializeField]
    private ActionType type;

    [SerializeField]
    private Destination destination;

    [SerializeField]
    private List<FarmEffect> farmEffects=new List<FarmEffect>();

    [SerializeField]
    private List<ContainerEffect> containerEffects=new List<ContainerEffect>();

    [SerializeField]
    private List<CharacterEffect> characterEffects=new List<CharacterEffect>();

    [SerializeField]
    private UpdateEffectOnPlayer playerEffect=UpdateEffectOnPlayer.NONE;

    [SerializeField]
    private InteractionType interactionType=InteractionType.NONE;

    public int Uses { get => uses; set => uses = value; }
    public ActionType Type { get => type; set => type = value; }
    public Destination Destination { get => destination; set => destination = value; }
    public List<FarmEffect> FarmEffects { get => farmEffects; set => farmEffects = value; }
    public List<ContainerEffect> ContainerEffects { get => containerEffects; set => containerEffects = value; }
    public List<CharacterEffect> CharacterEffects { get => characterEffects; set => characterEffects = value; }
    public UpdateEffectOnPlayer PlayerEffect { get => playerEffect; set => playerEffect = value; }
    public InteractionType InteractionType { get => interactionType; set => interactionType = value; }

    public bool NoNeedToChooseItemToApplyUpdateAction(){
        return this.InteractionType.Equals(InteractionType.ALL) || this.InteractionType.Equals(InteractionType.ANY);
    }


    public double GetComplexityOfAction(){
        double result=0.0;
        for(int i=0;i<Uses;i++){
            if(!Type.Equals(ActionType.UPDATE)){
                result+=1.5;
            }else{
                result+=Destination.Equals(Destination.PLAYER)?1.5:0.35*(ContainerEffects.Count+CharacterEffects.Count+FarmEffects.Count);
            }
        }
        return result;
    }
  
}
