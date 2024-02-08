using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenStats : CharacterStats
{

    public void InitQueenStats(System.Random random){
        InitVariables(random);
    }
    private void Start(){
        this.timeLastFrame=0f;
        this.allBarsManager=this.gameObject.GetComponentInChildren<AllBarsManager>();
        allBarsManager.healthBar.SetMaxBarValue(GetMaxHP());
    }

}
