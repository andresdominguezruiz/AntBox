using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public BattleMovement battleMovement;

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(battleMovement.actualTarget)){
            battleMovement.battleManager.inBattle=true;
        }
    }
    void OnTriggerStay2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(battleMovement.actualTarget)){
            battleMovement.battleManager.inBattle=true;
        }
        else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) battleMovement.otherAvailableTargets.Add(collider.transform);
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if(collider.gameObject.transform.Equals(battleMovement.actualTarget)){
            battleMovement.battleManager.inBattle=false;
        }else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) battleMovement.otherAvailableTargets.Remove(collider.transform);
        }
    }
}
