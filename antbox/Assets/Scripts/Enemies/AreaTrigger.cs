using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public BattleMovement battleMovement;

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(battleMovement.ActualTarget)){
            battleMovement.BattleManager.inBattle=true;
        }
    }
    void OnTriggerStay2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(battleMovement.ActualTarget)){
            battleMovement.BattleManager.inBattle=true;
        }
        else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) battleMovement.OtherAvailableTargets.Add(collider.transform);
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if(collider.gameObject.transform.Equals(battleMovement.ActualTarget)){
            battleMovement.BattleManager.inBattle=false;
        }else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) battleMovement.OtherAvailableTargets.Remove(collider.transform);
        }
    }
}
