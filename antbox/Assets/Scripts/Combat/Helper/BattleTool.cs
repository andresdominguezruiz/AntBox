using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType{
    ENTER,STAY,EXIT
}

public static class BattleTool
{

    public static void OnTriggerUpdater(Collider2D collider
    ,BattleMovement battleMovement, TriggerType trigger){
        bool battle=trigger.Equals(TriggerType.STAY) || trigger.Equals(TriggerType.ENTER);
        if(collider.gameObject.transform.Equals(battleMovement.ActualTarget)){
            battleMovement.BattleManager.inBattle=battle;
        }
        else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null && trigger.Equals(TriggerType.STAY)){
                battleMovement.OtherAvailableTargets.Add(collider.transform);
            }else if(item!=null && trigger.Equals(TriggerType.EXIT)){
                battleMovement.OtherAvailableTargets.Remove(collider.transform);
            }
        }
    }
    
}
