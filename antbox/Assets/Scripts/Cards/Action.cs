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
    ANT,CONTAINER,WATER_FARM,FOOD_FARM,QUEEN,FARM,ANTHILL
}

public enum UpdateEffectOnAntOrQueen{
    RESET_AGE,HP_UP,RESTORE_ENERGY,FEED,HYDRATE,RESTORE_HP,RESTORE_EVERYTHING,NONE
}
public enum UpdateEffectOnFarm{
    FARM_CYCLE_DOWN,MORE_CAPACITY,FARM_RESOURCES_UP,NONE
}
public enum UpdateEffectOnContainer{
    MORE_FOOD,MORE_WATER,NONE,DOUBLE_FOOD,DOUBLE_WATER,DOUBLE_EVERYTHING,MORE_EVERYTHING
}

[System.Serializable] //Al colocar esto, permitimos que Unity pueda interpretar esta clase y que pueda construirse
//esta clase desde su interfaz
public class Action
{
    public int uses=1;
    public ActionType type;
    public Destination destination;
    public UpdateEffectOnFarm farmEffect=UpdateEffectOnFarm.NONE;
    public UpdateEffectOnContainer containerEffect=UpdateEffectOnContainer.NONE;
    public UpdateEffectOnAntOrQueen characterEffect=UpdateEffectOnAntOrQueen.NONE;
    public InteractionType interactionType=InteractionType.NONE;

    public bool NoNeedToChooseItemToApplyUpdateAction(){
        return this.interactionType.Equals(InteractionType.ALL) || this.interactionType.Equals(InteractionType.ANY);
    }
        
}
