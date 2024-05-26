using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenStats : CharacterStats
{

    public void InitQueenStats(System.Random random){
        InitVariables(random);
    }
    private void Start(){
        this.TimeLastFrame=0f;
        this.AllBarsManager=this.gameObject.GetComponentInChildren<AllBarsManager>();
        AllBarsManager.HealthBar.SetMaxBarValue(GetMaxHP());
        AllBarsManager.HungerBar.SetMaxBarValue(GetMaxHunger());
        AllBarsManager.ThirstBar.SetMaxBarValue(GetMaxThirst());
    }

}
