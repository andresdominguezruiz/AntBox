using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public enum ActionType{
    ADD,UPDATE,UPDATE_ALL,UPDATE_ANY
}
public enum Destination{
    ANT,CONTAINER,WATER_FARM,FOOD_FARM,QUEEN
}
public enum UpdateEffect{
    RESET_AGE,HP_UP,FARM_CYCLE_DOWN,MORE_WATER,MORE_FOOD,NONE
}
[System.Serializable] //Al colocar esto, permitimos que Unity pueda interpretar esta clase y que pueda construirse
//esta clase desde su interfaz
public class Action
{
    public int uses=1;
    public ActionType type;
    public Destination destination;
    public UpdateEffect updateEffect=UpdateEffect.NONE;
        
}
