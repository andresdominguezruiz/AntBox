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
    ANT,CONTAINER,WATER_FARM,FOOD_FARM,QUEEN,FARM,ANTHILL,PLAYER
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
    public int uses=1;
    public ActionType type;
    public Destination destination;
    public List<FarmEffect> farmEffects=new List<FarmEffect>();
    public List<ContainerEffect> containerEffects=new List<ContainerEffect>();

    public List<CharacterEffect> characterEffects=new List<CharacterEffect>();
    public UpdateEffectOnPlayer playerEffect=UpdateEffectOnPlayer.NONE;
    public InteractionType interactionType=InteractionType.NONE;

    public bool NoNeedToChooseItemToApplyUpdateAction(){
        return this.interactionType.Equals(InteractionType.ALL) || this.interactionType.Equals(InteractionType.ANY);
    }


    public double GetComplexityOfAction(){
        double result=0.0;
        for(int i=0;i<uses;i++){
            if(!type.Equals(ActionType.UPDATE)){
                result+=0.5;
            }else{
                result+=destination.Equals(Destination.PLAYER)?1.0:0.2*(containerEffects.Count+characterEffects.Count+farmEffects.Count);
            }
        }
        return result;
    }
  
}
