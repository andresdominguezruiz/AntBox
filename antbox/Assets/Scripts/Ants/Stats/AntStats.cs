using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntStats : CharacterStats
{
    //------------------------------------
    private System.Random random = new System.Random();
    [SerializeField] protected int MIN_ENERGY=50;
    [SerializeField] protected int MAX_ENERGY=200;

    //----------------------------------------------------
    [SerializeField] private int maxEnergy;

    [SerializeField] private int actualEnergy;

    public string GetEnergyText(){
        return maxEnergy.ToString()+"/"+actualEnergy.ToString();
    }


    public void SetEnergy(int energy){
        actualEnergy=energy;
    }

    public void ApplyEnergyCost(int cost){
        SetEnergy(actualEnergy-cost);

    }

    private void Start(){
        InitOtherVariables();
        InitVariables();
    }

    public void InitOtherVariables(){
        int randomEnergy=random.Next(MIN_ENERGY,MAX_ENERGY);
        maxEnergy=randomEnergy;
        SetEnergy(randomEnergy);

    }
}
