using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum ActualAction{
    NOTHING,FARMING,SLEEPING,DIGGING,ATTACKING
}


public class AntStats : CharacterStats
{
    //------------------------------------

    private ActualAction action=ActualAction.NOTHING;
    [SerializeField] protected int MIN_ENERGY=50;
    [SerializeField] protected int MAX_ENERGY=200;
    [SerializeField] protected int ENERGY_PER_AGE=5;

    [SerializeField] protected int MIN_FARMING_SPEED=50; //es porcentual la velocidad
    [SerializeField] protected int MAX_FARMING_SPEED=200;
    [SerializeField] protected int FARMING_SPEED_PER_AGE=10;

    [SerializeField] protected int MIN_DIGGING_SPEED=10; //es porcentual la velocidad
    [SerializeField] protected int MAX_DIGGING_SPEED=50;
    [SerializeField] protected int DIGGING_SPEED_PER_AGE=5;

    [SerializeField] protected int MIN_RECOVER_SPEED=10; //es porcentual la velocidad
    [SerializeField] protected int MAX_RECOVER_SPEED=30;
    [SerializeField] protected int RECOVER_SPEED_PER_AGE=5;

    [SerializeField] protected int MIN_DAMAGE=5;
    [SerializeField] protected int MAX_DAMAGE=30;


    //La velocidad de ataque se calculará de forma porcentual
    [SerializeField] protected int MIN_ATTACK_SPEED=30;
    [SerializeField] protected int MAX_ATTACK_SPEED=240;

    [SerializeField] protected int MIN_MISS_PROBABILITY=10;
    [SerializeField] protected int MAX_MISS_PROBABILITY=35;


    [SerializeField] protected int MIN_CRITICAL_PROBABILITY=75;
    [SerializeField] protected int MAX_CRITICAL_PROBABILITY=100;


    //----------------------------------------------------
    [SerializeField] private int maxEnergy;

    [SerializeField] private int actualEnergy;
    [SerializeField] private float farmingSpeed;
    [SerializeField] private float diggingSpeed;
    [SerializeField] private int recoverSpeed;

    public BattleStats battleStats;
    //TODO: Cuando tengamos juego base, mejorar stats para que cada hormiga sea buena en algo

    public float GetFarmingSpeed(){
        return farmingSpeed;
    }

    public void SetFarmingSpeed(float speed){
        farmingSpeed=speed;
        if(speed>(float)MAX_FARMING_SPEED*2/100){
            farmingSpeed=(float)MAX_FARMING_SPEED*2/100;
        }
        else if(speed<(float)MIN_FARMING_SPEED/(2*100)){
            farmingSpeed=(float)MIN_FARMING_SPEED/(2*100);
        }
    }
    public void SetDiggingSpeed(float speed){
        diggingSpeed=speed;
        if(speed>(float)MAX_DIGGING_SPEED*2/100){
            diggingSpeed=(float)MAX_DIGGING_SPEED*2/100;
        }
        else if(speed<(float)MIN_DIGGING_SPEED/(2*100)){
            diggingSpeed=(float)MIN_DIGGING_SPEED/(2*100);
        }
    }

    public void SetRecoverSpeed(int speed){
        recoverSpeed=speed;
        if(speed>MAX_RECOVER_SPEED*2){
            recoverSpeed=MAX_RECOVER_SPEED*2;
        }
        else if(speed<MIN_RECOVER_SPEED/2){
            recoverSpeed=MIN_RECOVER_SPEED/2;
        }
    }

    public void CancelAntAction(){
        if(this.GetAction().Equals(ActualAction.FARMING)){
            StopFarming();
        }
        else if(this.GetAction().Equals(ActualAction.DIGGING)){
            StopDigging();
        }
        else if(this.GetAction().Equals(ActualAction.SLEEPING)){
            StopSleeping();
        }
        else if(this.GetAction().Equals(ActualAction.ATTACKING)){
            StopAttacking();
        }
    }

    public void StopAttacking(){
        BattleMovement battleMovement=this.GetComponent<BattleMovement>();
        if(battleMovement!=null) {
            battleMovement.KillingMode=false;
            battleMovement.BattleManager.inBattle=false;
            battleMovement.Agent.SetDestination(this.gameObject.transform.position);
        }
        this.DoNothing();
    }

    public void StopSleeping(){
        this.DoNothing();
    }

    public int GetActualEnergy(){
        return actualEnergy;
    }
    public int GetMaxEnergy(){
        return maxEnergy;
    }
    public bool IsFullOfEnergy(){
        return actualEnergy==maxEnergy;
    }

    public ActualAction GetAction(){
        return action;
    }
    public void StopFarming(){
        FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
        foreach(FarmStats farm in allFarms){
            if(farm.antsOfFarm.Contains(this.gameObject)){
                farm.antsOfFarm.Remove(this.gameObject);
                farm.antsWorkingInFarm.Remove(this.gameObject);
                this.DoNothing();
                this.gameObject.GetComponent<NavMeshAgent>().isStopped=false;
                this.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.gameObject.transform.position);
                break;
            }
        }
    }
    public void StopDigging(){
        this.gameObject.GetComponent<NavMeshAgent>().enabled=true;
        this.gameObject.GetComponent<ExcavationMovement>().StopDigging();
        this.DoNothing();
    }

    public void StartAttackingWithoutTarget(){
        BattleMovement battleMovement=this.GetComponent<BattleMovement>();
        if(battleMovement!=null){
            battleMovement.KillingMode=true;
            battleMovement.BattleManager.inBattle=true;
            action=ActualAction.ATTACKING;
        }
    }

    public void StartAttacking(Transform firstTarget){
        BattleMovement battleMovement=this.GetComponent<BattleMovement>();
        if(battleMovement!=null){
            battleMovement.KillingMode=true;
            battleMovement.ActualTarget=firstTarget;
            battleMovement.BattleManager.inBattle=true;
            action=ActualAction.ATTACKING;
        }

    }
    public void StartFarming(){
        action=ActualAction.FARMING;
    }

    public void StartDigging(){
        action=ActualAction.DIGGING;
    }
    public void GoToSleep(){
        action=ActualAction.SLEEPING;
    }

    public void DoNothing(){
        action=ActualAction.NOTHING;
    }

    public string GetEnergyText(){
        return maxEnergy.ToString()+"/"+actualEnergy.ToString();
    }

    public float GetDiggingSpeed(){
        return diggingSpeed;
    }


    public void SetEnergy(int energy){
        if(energy<0){
            actualEnergy=0;
        }
        else if(energy>maxEnergy){
            actualEnergy=maxEnergy;
        }
        else{
            actualEnergy=energy;
        }
    }
    public void SetMaxEnergy(int energy){
        if(energy<MIN_ENERGY/2){
            maxEnergy=MIN_ENERGY/2;
        }
        else if(energy>MAX_ENERGY*2){
            maxEnergy=MAX_ENERGY*2;
        }
        else{
            maxEnergy=energy;
        }
    }

    public void ApplyEnergyCost(int cost){
        if(!GetClockOfGame().eventType.Equals(EventType.SUMMER) || cost<0){
            SetEnergy(actualEnergy-cost);
        }
        else {
            SetEnergy(actualEnergy-2*cost);
        }
        AllBarsManager.EnergyBar.SetBarValue(actualEnergy);

    }
    public void InitAntStats(System.Random random){
        InitVariables(random);
        InitOtherVariables();
    }
    public int GetRecoverSpeed(){
        return recoverSpeed;
    }

    private void Start(){
        this.timeLastFrame=0f;
        this.AllBarsManager = this.gameObject.GetComponentInChildren<AllBarsManager>();
        AllBarsManager.HealthBar.SetMaxBarValue(GetMaxHP());
        AllBarsManager.EnergyBar.SetMaxBarValue(GetMaxEnergy());
        AllBarsManager.HungerBar.SetMaxBarValue(GetMaxHunger());
        AllBarsManager.ThirstBar.SetMaxBarValue(GetMaxThirst());
    }

    public void InitOtherVariables(){
        int randomEnergy=GetRandom().Next(MIN_ENERGY,MAX_ENERGY);
        int randomSpeed=GetRandom().Next(MIN_FARMING_SPEED,MAX_FARMING_SPEED);
        int randomDigging=GetRandom().Next(MIN_DIGGING_SPEED,MAX_DIGGING_SPEED);
        int randomRecover=GetRandom().Next(MIN_RECOVER_SPEED,MAX_RECOVER_SPEED);
        int randomDamage=GetRandom().Next(MIN_DAMAGE,MAX_DAMAGE);
        int randomAttackSpeed=GetRandom().Next(MIN_ATTACK_SPEED,MAX_ATTACK_SPEED);
        int randomMissProbability=GetRandom().Next(MIN_MISS_PROBABILITY,MAX_MISS_PROBABILITY);
        int randomCriticalProbability=GetRandom().Next(MIN_CRITICAL_PROBABILITY,MAX_CRITICAL_PROBABILITY);
        farmingSpeed=(float)(randomSpeed*1.0/100);
        diggingSpeed=(float)(randomDigging*1.0/100);
        battleStats=new BattleStats(false,randomDamage,randomAttackSpeed,randomMissProbability,randomCriticalProbability);
        maxEnergy=randomEnergy;
        recoverSpeed=randomRecover;
        SetEnergy(randomEnergy);

    }
}
