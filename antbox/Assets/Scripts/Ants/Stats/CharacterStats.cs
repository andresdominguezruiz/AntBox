using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public enum AgeType{
    YOUNG,ADULT,ELDER
}

public class CharacterStats : MonoBehaviour
{
    private System.Random random = new System.Random();

    public System.Random GetRandom(){
        return random;
    }
    private float timeLastFrame;


    [SerializeField] private static int growingTime=24; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;

    [SerializeField]
    private AllBarsManager allBarsManager;


    //LIMITS FOR VARIABLES----------------------
    [SerializeField] protected int MIN_HP=40;
    [SerializeField] protected int MAX_HP=60;
    [SerializeField] protected int HP_PER_AGE=5;

    [SerializeField] protected int MIN_HUNGER=50;
    [SerializeField] protected int MAX_HUNGER=100;
    [SerializeField] protected int HUNGER_PER_AGE=5;

    [SerializeField] protected int MIN_THIRST=50;

    [SerializeField] protected int MAX_THIRST=100;
    [SerializeField] protected int THIRST_PER_AGE=5;


    //----------------------------
    [SerializeField] private int maxHP;
    [SerializeField] private int actualHP;

    [SerializeField] private int maxHunger;
    [SerializeField] private int actualHunger;

    [SerializeField] private int maxThirst;
    [SerializeField] private int actualThirst;

    [SerializeField] private int age;

    [SerializeField] private bool isDead=false;
    [SerializeField] private int adultAge=4;
    [SerializeField] private int elderAge=8;
    private int poisonSecons = 0;
    private bool unpoisonable = false;
    private Clock clockOfGame;

    public AllBarsManager AllBarsManager { get => allBarsManager; set => allBarsManager = value; }
    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int ActualHP { get => actualHP; set => actualHP = value; }
    public int MaxHunger { get => maxHunger; set => maxHunger = value; }
    public int ActualHunger { get => actualHunger; set => actualHunger = value; }
    public int MaxThirst { get => maxThirst; set => maxThirst = value; }
    public int ActualThirst { get => actualThirst; set => actualThirst = value; }
    public int PoisonSecons { get => poisonSecons; set => poisonSecons = value; }
    public bool Unpoisonable { get => unpoisonable; set => unpoisonable = value; }
    public float TimeLastFrame { get => timeLastFrame; set => timeLastFrame = value; }

    public Clock GetClockOfGame(){
        return clockOfGame;
    }

    public void SetClockOfGame(Clock clock){
        clockOfGame=clock;
    }

    public bool IsDead(){
        return isDead;
    }

    
    void Update(){
        if(Time.time -TimeLastFrame>=(1.0f+Player.Instance.GetTimeValue()-StatisticsOfGame.Instance.timeSpeed*0.1f)){
            counterOfSecons++;
            if(counterOfSecons==growingTime){
                age++;
                UpdateStatsPerAge();
                counterOfSecons=0;
            }
            UpdateStats();
            TimeLastFrame=Time.time;
        }
    }

    void UpdateStatsPerAge(){
        if(age<=adultAge){
            MaxHP+=HP_PER_AGE;
            MaxHunger+=HUNGER_PER_AGE;
            MaxThirst+=THIRST_PER_AGE;
        }else if(age>=elderAge && Player.Instance.CanAnthillDieByOldAge()){
            MaxHP-=HP_PER_AGE;
            MaxHunger-=HUNGER_PER_AGE/2;
            MaxThirst-=THIRST_PER_AGE/2;
        }
    }

    void KillAnt(){
        SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
        item.IsSelected=false;
        if(this.gameObject.GetComponent<AntStats>()!=null){
            FarmStats[] farms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in farms){
                if(farm.antsOfFarm.Contains(this.gameObject)){
                    farm.antsOfFarm.Remove(this.gameObject);
                    farm.antsWorkingInFarm.Remove(this.gameObject);
                }
            }
        }
        BattleMovement[] battleManagers=FindObjectsOfType<BattleMovement>(false);
        foreach(BattleMovement manager in battleManagers){
            manager.OtherAvailableTargets.Remove(this.transform);
        } 
        item.RemoveSelectableItem();
        AntsTool.IsEndOfGame();
        Destroy(this.gameObject);
    }

    void UpdateStats(){
        AntStats antStats=this.gameObject.GetComponent<AntStats>();
        if(isDead){
            KillAnt();
        }else{
            bool needToCheckHP=false;
            int cost=-1;
            if(ActualHunger>0 && ActualThirst>0){
                if(!(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER))){
                    cost= -3;
                }
                SetActualHunger(ActualHunger+cost);
                SetActualThirst(ActualThirst+cost);
                Heal(1);
            }else if(ActualHunger>0){
                SetActualHunger(ActualHunger+cost);
                ActualHP--;
                needToCheckHP=true;
            }else if(ActualThirst>0){
                SetActualThirst(ActualThirst+cost);
                ActualHP--;
                needToCheckHP=true;
            }else{
                ActualHP-=2;
                needToCheckHP=true;
            }
            if(PoisonSecons>0){
                PoisonSecons--;
                if(!Unpoisonable){
                    Heal(-ActualHP*5/100); //EL VENENO LE QUITARÁ 5% DE SU VIDA
                    needToCheckHP=true;
                }
                else{
                    Heal(ActualHP*5/100);
                }
            }

            if(antStats!=null){
                if(antStats.GetAction().Equals(ActualAction.SLEEPING)){
                    antStats.ApplyEnergyCost(-1*antStats.GetRecoverSpeed());
                    if(antStats.IsFullOfEnergy()){
                        antStats.CancelAntAction();
                    }
                }
            }
            if(ActualThirst<0){
                ActualThirst=0;
            }
            if(ActualHunger<0){
                ActualHunger=0;
            }

            if(needToCheckHP){
                CheckHP();
            }
        }
    }
    public void EatWithoutCost(int foodValue){
        if(MaxHunger<ActualHunger+foodValue){
            SetActualHunger(MaxHunger);
        }
        else{
            SetActualHunger(ActualHunger+foodValue);
        }
    }
    public void DrinkWithoutCost(int waterValue){
        if(MaxThirst<ActualThirst+waterValue){
            SetActualThirst(MaxThirst);
        }
        else{
            SetActualThirst(ActualThirst+waterValue);
        }
    }

    public void Eat(ContainerData container){
        if(container.FOOD_CONTAINER>0){
            container.FOOD_CONTAINER--;
            if(MaxHunger<ActualHunger+container.FoodValue){
                SetActualHunger(MaxHunger);
            }
            else{
                SetActualHunger(ActualHunger+container.FoodValue);
            }
        }
    }
    public void Drink(ContainerData container){
        if(container.WATER_CONTAINER>0){
            container.WATER_CONTAINER--;
            if(MaxThirst<ActualThirst+container.WaterValue){
                SetActualThirst(MaxThirst);
            }
            else{
                SetActualThirst(ActualThirst+container.WaterValue);
            }
        }
    }
    public void CheckHP(){
        if(ActualHP<=0 || MaxHP==0){
            ActualHP=0;
            Die();
        }else if(ActualHP>=MaxHP){
            ActualHP=MaxHP;
        }
        if(AllBarsManager!=null && AllBarsManager.HealthBar!=null){
            AllBarsManager.HealthBar.SetBarValue(ActualHP);
        }
    }

    public void Die(){
        isDead=true;
    }

    public void TakeDamage(int damage){
        SetActualHP(ActualHP-damage);
        
    }
    public void IncrementHP(int extraHP){
        MaxHP+=extraHP;
        Heal(MaxHP);
    }

    public void Heal(int extraHp){
        if(extraHp+ActualHP>MaxHP){
            SetActualHP(MaxHP);
        }
        else{
            SetActualHP(extraHp+ActualHP);
        }
    }

    public int GetMaxHP(){
        return MaxHP;
    }

    public void SetActualHP(int hp){
        ActualHP=hp;
        if(AllBarsManager!=null && AllBarsManager.HealthBar!=null){
            AllBarsManager.HealthBar.SetBarValue(ActualHP);
        }
    }

    public void SetActualHunger(int hunger){
        if(hunger<0){
            ActualHunger=0;
        }else{
            ActualHunger=hunger;
        }
        if(AllBarsManager!=null && AllBarsManager.HungerBar!=null){
            AllBarsManager.HungerBar.SetBarValue(ActualHunger);
        }
    }

    public void SetActualThirst(int thirst){
        if(thirst<0){
            ActualThirst=0;
        }else{
            ActualThirst=thirst;
        }
        if(AllBarsManager!=null && AllBarsManager.ThirstBar!=null){
            AllBarsManager.ThirstBar.SetBarValue(ActualThirst);
        }
    }

    public String GetTextHP(){
        String text=ActualHP.ToString()+'/'+MaxHP.ToString();
        return text;
    }
    public string GetTextHunger(){
        String text=ActualHunger.ToString()+'/'+MaxHunger.ToString();
        return text;
    }
    public string GetTextThirst(){
        String text=ActualThirst.ToString()+'/'+MaxThirst.ToString();
        return text;
    }

    public string GetTextAge(){
        return age.ToString();
    }

    public void ProcessUpdateEffectOfAction(List<CharacterEffect> effects){
        foreach(CharacterEffect effect in effects){
            ApplyEffect(effect);
        }
    }

    public void ApplyEffect(CharacterEffect effect){
        AntStats stats=this.gameObject.GetComponent<AntStats>();
        if(!effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.NONE)){
            if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HP_LIMIT)) {
                MaxHP=effect.MultiplicatorValue*(MaxHP+(int)effect.SumValue);
                if(MaxHP<MIN_HP/2){
                    MaxHP=MIN_HP/2;
                }
                else if(MaxHP>MAX_HP*2){
                    MaxHP=MAX_HP*2;
                }
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.AGE)){
                age=effect.MultiplicatorValue*(age+(int)effect.SumValue);
                if(age<0 && !Player.Instance.AllowNegativeAge()){
                    age=0;
                }
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.FEED)){
                this.EatWithoutCost(effect.MultiplicatorValue*ActualHunger+(int)effect.SumValue);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HYDRATE)){
                this.DrinkWithoutCost(effect.MultiplicatorValue*ActualThirst+(int)effect.SumValue);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_ENERGY) && stats!=null){
                stats.SetEnergy(stats.GetMaxEnergy());
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_HP)){
                this.SetActualHP(MaxHP);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_HUNGER)){
                this.SetActualHunger(MaxHunger);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_THIRST)){
                this.SetActualThirst(MaxThirst);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HUNGER_LIMIT)){
                MaxHunger=effect.MultiplicatorValue*(MaxHunger+(int)effect.SumValue);
                if(MaxHunger<MIN_HUNGER/2) {
                    MaxHunger=MIN_HUNGER/2;
                }
                else if(MaxHunger>MAX_HUNGER*2){
                    MaxHunger=MAX_HUNGER*2;
                }
            }else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.THIRST_LIMIT)){
                MaxThirst=effect.MultiplicatorValue*(MaxThirst+(int)effect.SumValue);
                if(MaxThirst<MIN_THIRST/2) {
                    MaxThirst=MIN_THIRST/2;
                }
                else if(MaxThirst>MAX_THIRST*2) {
                    MaxThirst=MAX_THIRST*2;
                }
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.ENERGY_LIMIT) && stats!=null){
                stats.SetMaxEnergy(effect.MultiplicatorValue*(stats.GetMaxEnergy()+(int)effect.SumValue));
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.DIGGING_SPEED) && stats!=null){
                stats.SetDiggingSpeed((float)effect.MultiplicatorValue*(stats.GetDiggingSpeed()+effect.SumValue));
            }else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.FARMING_SPEED) && stats!=null){
                stats.SetFarmingSpeed((float)effect.MultiplicatorValue*(stats.GetFarmingSpeed()+effect.SumValue));
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RECOVER_SPEED) && stats!=null){
                stats.SetRecoverSpeed(effect.MultiplicatorValue*(stats.GetRecoverSpeed()+(int)effect.SumValue));
            }

        }
    }

    public int GetMaxHunger(){
        return MaxHunger;
    }

    public int GetMaxThirst(){
        return MaxThirst;
    }



    public void InitVariables(System.Random random){
        Clock clock=FindObjectOfType<Clock>();
        SetClockOfGame(clock);
        this.random=random;
        int randomHP=random.Next(MIN_HP,MAX_HP);
        int randomHunger=random.Next(MIN_HUNGER,MAX_HUNGER);
        int randomThirst=random.Next(MIN_THIRST,MAX_THIRST);

        MaxHP=randomHP;
        ActualHP=randomHP;
        MaxHunger=randomHunger;
        ActualHunger=randomHunger;
        MaxThirst=randomThirst;
        ActualThirst=randomThirst;

        isDead=false;
        age=0;
    }
}
