using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActualAction{
    NOTHING,FARMING,SLEEPING,DIGGING
}


public class AntStats : CharacterStats
{
    //------------------------------------
    private System.Random random = new System.Random();
    private ActualAction action=ActualAction.NOTHING;
    [SerializeField] protected int MIN_ENERGY=50;
    [SerializeField] protected int MAX_ENERGY=200;

    [SerializeField] protected int MIN_FARMING_SPEED=50; //es porcentual la velocidad
    [SerializeField] protected int MAX_FARMING_SPEED=200;


    //----------------------------------------------------
    [SerializeField] private int maxEnergy;

    [SerializeField] private int actualEnergy;
    [SerializeField] private float farmingSpeed;
    //TODO: Cuando tengamos juego base, mejorar stats para que cada hormiga sea buena en algo

    public float GetFarminSpeed(){
        return farmingSpeed;
    }

    public ActualAction GetAction(){
        return action;
    }
    public void StartFarming(){
        action=ActualAction.FARMING;
    }

    public void DoNothing(){
        action=ActualAction.NOTHING;
    }

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
        this.timeLastFrame=0f;
    }

    public void InitOtherVariables(){
        int randomEnergy=random.Next(MIN_ENERGY,MAX_ENERGY);
        int randomSpeed=random.Next(MIN_FARMING_SPEED,MAX_FARMING_SPEED);
        farmingSpeed=(float)(randomSpeed*1.0/100);
        maxEnergy=randomEnergy;
        SetEnergy(randomEnergy);

    }
}
