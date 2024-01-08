using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class CharacterStats : MonoBehaviour
{
    private System.Random random = new System.Random();

    public System.Random GetRandom(){
        return random;
    }
    public float timeLastFrame;


    [SerializeField] private static int growingTime=24; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;

    //LIMITS FOR VARIABLES----------------------
    [SerializeField] protected int MIN_HP=40;
    [SerializeField] protected int MAX_HP=60;
    [SerializeField] protected int HP_PER_AGE=2;

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
    [SerializeField] private int adultAge=3;
    private Clock clockOfGame;

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
        if(Time.time -timeLastFrame>=1.0f){
            counterOfSecons++;
            if(counterOfSecons==growingTime){
                age++;
                //UpdateStatsPerAge();
                counterOfSecons=0;
            }
            UpdateStats();
            timeLastFrame=Time.time;
        }
    }

    void UpdateStatsPerAge(){
        if(age<=adultAge){
            maxHP+=HP_PER_AGE;
            maxHunger+=HUNGER_PER_AGE;
            maxThirst+=THIRST_PER_AGE;
        }
    }

    void UpdateStats(){
        AntStats antStats=this.gameObject.GetComponent<AntStats>();
        if(isDead){
            SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
            item.isSelected=false;
            FarmStats[] farms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in farms){
                if(farm.antsOfFarm.Contains(this.gameObject)){
                    farm.antsOfFarm.Remove(this.gameObject);
                    farm.antsWorkingInFarm.Remove(this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }else{
            bool needToCheckHP=false;
            if(actualHunger>0 && actualThirst>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)){
                    actualHunger--;
                    actualThirst--;
                }else{
                    actualHunger-=3;
                    actualThirst-=3;
                }
                Heal(1);
            }else if(actualHunger>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)) actualHunger--;
                else actualHunger-=3;
                actualHP--;
                needToCheckHP=true;
            }else if(actualThirst>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)) actualThirst--;
                else actualThirst-=3;
                actualHP--;
                needToCheckHP=true;
            }else{
                actualHP-=2;
                needToCheckHP=true;
            }

            if(antStats!=null){
                if(antStats.GetAction().Equals(ActualAction.SLEEPING)){
                    antStats.ApplyEnergyCost(-1*antStats.GetRecoverSpeed());
                    if(antStats.IsFullOfEnergy()) antStats.CancelAntAction();
                }
            }

            if(needToCheckHP) CheckHP();
        }
    }
    public void EatWithoutCost(int foodValue){
        if(maxHunger<actualHunger+foodValue) SetActualHunger(maxHunger);
        else SetActualHunger(actualHunger+foodValue);
    }
    public void DrinkWithoutCost(int waterValue){
        if(maxThirst<actualThirst+waterValue) SetActualThirst(maxThirst);
        else SetActualThirst(actualThirst+waterValue);
    }

    public void Eat(ContainerData container){
        if(container.FOOD_CONTAINER>0){
            container.FOOD_CONTAINER--;
            Debug.Log("ha comido");
            if(maxHunger<actualHunger+container.foodValue) SetActualHunger(maxHunger);
            else SetActualHunger(actualHunger+container.foodValue);
        }else{
            Debug.Log("no ha comido");
        }
    }
    public void Drink(ContainerData container){
        if(container.WATER_CONTAINER>0){
            container.WATER_CONTAINER--;
            if(maxThirst<actualThirst+container.waterValue) SetActualThirst(maxThirst);
            else SetActualThirst(actualThirst+container.waterValue);
        }
    }




    public void CheckHP(){
        if(actualHP<=0){
            actualHP=0;
            Die();
        }else if(actualHP>=maxHP){
            actualHP=maxHP;
        }
    }

    public void Die(){
        isDead=true;
    }

    public void TakeDamage(int damage){
        SetActualHP(actualHP-damage);
    }
    public void IncrementHP(int extraHP){
        maxHP+=extraHP;
        Heal(maxHP);
    }

    public void Heal(int extraHp){
        if(extraHp+actualHP>maxHP) SetActualHP(maxHP);
        else SetActualHP(extraHp+actualHP);
    }

    public int GetMaxHP(){
        return maxHP;
    }

    public void SetActualHP(int hp){
        actualHP=hp;
    }

    public void SetActualHunger(int hunger){
        actualHunger=hunger;
    }

    public void SetActualThirst(int thirst){
        actualThirst=thirst;
    }

    public String GetTextHP(){
        String text=maxHP.ToString()+'/'+actualHP.ToString();
        return text;
    }
    public string GetTextHunger(){
        String text=maxHunger.ToString()+'/'+actualHunger.ToString();
        return text;
    }
    public string GetTextThirst(){
        String text=maxThirst.ToString()+'/'+actualThirst.ToString();
        return text;
    }

    public string GetTextAge(){
        return age.ToString();
    }

    public void ProcessUpdateEffectOfAction(Action actualAction){
        AntStats stats=this.gameObject.GetComponent<AntStats>();
        if(!actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.NONE)){
            if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.HP_UP)) {
                //Incrementa segun porcentaje de la hormiga
                this.IncrementHP(this.GetMaxHP()/5);
            }
            else if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESET_AGE)){
                this.age=0;
            }
            else if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.FEED)){
                this.EatWithoutCost(maxHunger/2);
            }
            else if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.HYDRATE)){
                this.DrinkWithoutCost(maxThirst/2);
            }
            else if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_ENERGY) && stats!=null){
                stats.SetEnergy(stats.GetMaxEnergy());
            }
            else if(actualAction.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_EVERYTHING)){
                this.SetActualHP(maxHP);
                this.SetActualHunger(maxHunger);
                this.SetActualThirst(maxThirst);
                if(stats!=null) stats.SetEnergy(stats.GetMaxEnergy());
            }
        }
    }

    public void InitVariables(System.Random random){
        Clock clock=FindObjectOfType<Clock>();
        SetClockOfGame(clock);
        this.random=random;
        int randomHP=random.Next(MIN_HP,MAX_HP);
        int randomHunger=random.Next(MIN_HUNGER,MAX_HUNGER);
        int randomThirst=random.Next(MIN_THIRST,MAX_THIRST);

        maxHP=randomHP;
        actualHP=randomHP;
        maxHunger=randomHunger;
        actualHunger=randomHunger;
        maxThirst=randomThirst;
        actualThirst=randomThirst;

        isDead=false;
        age=0;
    }
}
